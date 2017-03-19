using System;

namespace CoreGame.Models.API {
    [Serializable]
    public class ResultInfo {
        protected static int SUCCESS_CODE = 0;

        public int ResultCode;
        public string ResultMessage;

        public bool IsSuccess() { return ResultCode == SUCCESS_CODE; }
    }
}