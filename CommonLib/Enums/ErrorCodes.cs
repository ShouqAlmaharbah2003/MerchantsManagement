namespace CommonLib.Enums
{
    public enum ErrorCodes
    {
        None = 0,               // لا يوجد
        GroupNotFound = 404,    // Not Found
        InvalidRequest = 400,   // Bad Request
        InternalServerError = 500,// Internal Server Error
        Unauthorized = 401,      // Unauthorized
        Forbidden = 403,
        ValidationError = 501,
    }
}