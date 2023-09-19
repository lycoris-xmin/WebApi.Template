using Lycoris.Autofac.Extensions;

namespace Lycoris.Api.Common.Snowflakes.Impl
{
    /// <summary>
    /// 雪花Id生成器
    /// </summary>
    [AutofacRegister(ServiceLifeTime.Singleton)]
    public class SnowflakesMaker : ISnowflakesMaker
    {
        readonly object locker = new();

        /// <summary>
        /// 最后的时间戳
        /// </summary>
        private long lastTimestamp = -1L;

        /// <summary>
        /// 最后的序号
        /// </summary>
        private uint lastIndex = 0;

        /// <summary>
        /// 工作机器长度，最大支持1024个节点，可根据实际情况调整，比如调整为9，则最大支持512个节点，可把多出来的一位分配至序号，提高单位毫秒内支持的最大序号
        /// </summary>
        private readonly int _workIdLength;

        /// <summary>
        /// 支持的最大工作节点
        /// </summary>
        private readonly int _maxWorkId;

        /// <summary>
        /// 序号长度，最大支持4096个序号
        /// </summary>
        private readonly int _indexLength;

        /// <summary>
        /// 支持的最大序号
        /// </summary>
        private readonly int _maxIndex;

        /// <summary>
        /// 当前工作节点
        /// </summary>
        private int? _workId;

        /// <summary>
        /// ctor
        /// </summary>
        public SnowflakesMaker()
        {
            _workIdLength = AppSettings.Snowflake.WorkIdLength;
            _maxWorkId = 1 << _workIdLength;

            // 工作机器id和序列号的总长度是22位，为了使组件更灵活，根据机器id的长度计算序列号的长度。
            _indexLength = 22 - _workIdLength;
            _maxIndex = 1 << _indexLength;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public long GetSnowflakesId(int? workId = null)
        {
            if (workId != null)
                _workId = workId.Value;

            if (_workId > _maxWorkId)
                throw new ArgumentException($"机器码取值范围为0-{_maxWorkId}");

            lock (locker)
            {
                if (_workId == null)
                    ChangeWorkId();

                var currentTimeStamp = SnowflakeTimeStamp(AppSettings.Snowflake.StartTime.Ticks);
                // 如果当前序列号大于允许的最大序号，则表示，当前单位毫秒内，序号已用完，则获取时间戳。
                if (lastIndex >= _maxIndex)
                    currentTimeStamp = SnowflakeTimeStamp(AppSettings.Snowflake.StartTime.Ticks, lastTimestamp);

                if (currentTimeStamp > lastTimestamp)
                {
                    lastIndex = 0;
                    lastTimestamp = currentTimeStamp;
                }
                else if (currentTimeStamp < lastTimestamp)
                {
                    // throw new Exception("时间戳生成出现错误");
                    // 发生时钟回拨，切换workId，可解决。
                    ChangeWorkId();
                    return GetSnowflakesId(workId);
                }

                if (_workId == null)
                    throw new ArgumentException(nameof(_workId));

                var work = _workId.Value << _indexLength;

                var time = currentTimeStamp << _indexLength + _workIdLength;

                var id = time | (long)work | lastIndex;

                lastIndex++;

                return id;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void ChangeWorkId()
        {
            lock (locker)
            {
                _workId ??= 0;
                if (_workId > 1 << AppSettings.Snowflake.WorkIdLength)
                    _workId++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private long SnowflakeTimeStamp(long ticks, long lastTimestamp = 0L)
        {
            var current = (DateTime.Now.Ticks - ticks) / 10000;
            return lastTimestamp == current ? SnowflakeTimeStamp(lastTimestamp) : current;
        }
    }
}
