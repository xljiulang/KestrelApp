namespace System.Buffers
{
    /// <summary>
    /// 提供ArrayPool的扩展
    /// </summary>
    public static class ArrayPoolExtensions
    {
        /// <summary>
        /// 申请可回收的IArrayOwner
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayPool"></param>
        /// <param name="length">有效数据长度</param>
        /// <returns></returns>
        public static IArrayOwner<T> RentArrayOwner<T>(this ArrayPool<T> arrayPool, int length)
        {
            return new ArrayOwner<T>(arrayPool, length);
        }       
    }
}
