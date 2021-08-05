namespace Dataway_Worker
{
    //TODO: put result somewhere better like in client class
    //TODO: make more result types
    internal class Result
    {
        public int code;
        public string message;

        //NEGATIVE == SOCKET ERROR
        //POSITVE == SERVER/CLIENT ERROR or RESPONSE
        public enum CODE
        {
            SOCKET_NOT_CONNECTED = -2,
            CONNECTION_REFUSED = -1,
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
                case CODE.SOCKET_NOT_CONNECTED:
                    return "No active connection to Dataway server";

                case CODE.CONNECTION_REFUSED:
                    return "Dataway server connection refused";

                case CODE.SUCCESS:
                    return "Success";

                case CODE.BAD_LOGIN:
                    return "Bad login";

                case CODE.DECLINED_TRANSMIT_REQUEST:
                    return "Transmit request was declined by client";

                case CODE.RECIEVER_OFFLINE:
                    return "The specified receiver is currently offline";

                case CODE.USER_LOGGED_OUT:
                    return "You need to login first";


                default:
                    return "Unknown result type";
            }
        }
    }
}