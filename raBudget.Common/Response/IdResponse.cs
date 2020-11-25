namespace raBudget.Common.Response
{
    public abstract class IdResponse<T> : BaseResponse
    {
        public T Id { get; set; }
    }
}