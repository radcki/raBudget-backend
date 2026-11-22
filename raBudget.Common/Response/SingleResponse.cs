namespace raBudget.Common.Response
{
    public abstract class SingleResponse<T> : BaseResponse
    {
        public T Data { get; set; }
    }
}
