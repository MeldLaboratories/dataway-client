namespace Dataway_Worker
{
    public class Result
    {
        public int code;
        public string message;

        public enum CODE
        {
            SUCCESS = 0,
            BAD_LOGIN = 1,
            DECLINED_TRANSMIT_REQUEST = 2,
            RECIEVER_OFFLINE = 3,
            USER_LOGGED_OUT = 4
        }

        public Result(CODE code)
        {
            this.code = ((int)code);
            this.message = GetErrorMessage(code);
        }

        private string GetErrorMessage(CODE code)
        {
            switch (code)
            {
                case CODE.SUCCESS:
                    return "Success";

                case CODE.BAD_LOGIN:
                    return "Bad login";

                case CODE.DECLINED_TRANSMIT_REQUEST:
                    return "Transmit request was declined by client";

                default:
                    return "Unknown error type";
            }
        }
    }
}