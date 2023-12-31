﻿/**
 * Copyright (C) 2017  (http://coding4ever.net/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * The latest version of this file can be found at https://github.com/rudi-krsoftware/spark-pos
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace SparkPOS.WebAPI.Models
{
    public class ResponsePackage
    {
        public ResponsePackage()
        {
            Status = new Status();
            Results = new List<object>();
        }

        public ResponsePackage(HttpStatusCode httpStatusCode)
            : this()
        {
            Status.Code = Convert.ToInt32(httpStatusCode);
            Status.Description = httpStatusCode.ToString();
        }

        public ResponsePackage(List<string> errors)
            : this()
        {
            Status.Code = Convert.ToInt32(HttpStatusCode.BadRequest);
            Status.Description = HttpStatusCode.BadRequest.ToString();
            Status.Errors = errors;
        }

        public Status Status { get; set; }
        public object Results { get; set; }        
    }
}