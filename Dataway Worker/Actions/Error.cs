namespace Dataway_Worker.Actions
{
    internal class Error
    {
        /// <summary>
        /// Creates a new error instance
        /// </summary>
        /// <param name="result">Result</param>
        /// <returns></returns>
        public static Formats.Generic.Error CreateError(Result res)
        {
            var response = new Formats.Generic.Error();
            response.Code = res.code;
            response.Text = res.message;
            return response;
        }
    }
}