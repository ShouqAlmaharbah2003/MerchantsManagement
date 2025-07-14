namespace CommonLib.Enums
{
    public enum ErrorCodes
    {
        None = 0,               // لا يوجد
        NotFound = 404, 
        InvalidRequest = 400,   // Bad Request
        InternalServerError = 500,
        Unauthorized = 401,      
        Forbidden = 403,
        ValidationError = 501,
    }
}