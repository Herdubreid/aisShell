﻿namespace Celin
{
    public class FormCtx : RequestCtx<AIS.FormRequest, FormCtx>
    {
        public FormCtx(string id) : base("fm", id)
        {
            Request.formServiceAction = "R";
        }
    }
}