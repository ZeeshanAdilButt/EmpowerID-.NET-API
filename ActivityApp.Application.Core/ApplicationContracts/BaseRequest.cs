namespace ActivityApp.Application.Core.ApplicationContracts
{
    public class BasePaginatedRequest : BaseRequest
    {
        int ? page;
        public int? pageSize;

    }

}
