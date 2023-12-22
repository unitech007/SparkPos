/**
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
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SparkPOS.Model
{
    public enum GrantState
    {
        SELECT, CREATE, UPDATE, DELETE
    }

	[Table("m_user")]
    public class User
    {
		[ExplicitKey]
		[Display(Name = "user_id")]		
		public string user_id { get; set; }
		
		[Display(Name = "role_id")]
		public string role_id { get; set; }

        [JsonIgnore]
		[Write(false)]
        public Role Role { get; set; }

		[Display(Name = "User Name")]
		public string name_user { get; set; }
		
		[Display(Name = "Password")]
		public string user_password { get; set; }

        [Write(false)]
        [Display(Name = "Konfirmasi Password")]
        public string konf_user_password { get; set; }

		[Display(Name = "Active")]
		public bool is_active { get; set; }

        [Write(false)]
		[Display(Name = "Status User")]
		public int status_user { get; set; }

        [Write(false)]
        public bool is_administrator
        {
            get
            {
                if (this.Role != null)
                {
                    return this.Role.name_role.ToLower() == "administrator";
                }
                else
                    return false;
            }
        }

        [JsonIgnore]
        [Write(false)]
        public IList<RolePrivilege> role_privileges { get; set; }

        public IList<RolePrivilege> GetRoleByMenu(string menuId)
        {
            var oList = this.role_privileges.Where(f => f.role_id == this.role_id && f.menu_id == menuId)
                                            .ToList();

            return oList;
        }

        public RolePrivilege GetRoleByMenuAndGrant(string menuId, GrantState grantState)
        {
            var obj = this.role_privileges.Where(f => f.role_id == this.role_id && f.menu_id == menuId && f.grant_id == Convert.ToInt32(grantState))
                                          .SingleOrDefault();

            return obj;
        }

        public RolePrivilege GetRoleByMenuNameAndGrant(string menuName, GrantState grantState)
        {
            var obj = this.role_privileges.Where(f => f.role_id == this.role_id && f.Menu.name_menu == menuName && f.grant_id == Convert.ToInt32(grantState))
                                          .SingleOrDefault();

            return obj;
        }

	}

    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

			var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

			RuleFor(c => c.role_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
			RuleFor(c => c.name_user).NotEmpty().WithMessage(msgError1).Length(1, 50).WithMessage(msgError2);
			RuleFor(c => c.user_password).NotEmpty().WithMessage(msgError1).Length(1, 32).WithMessage(msgError2);
            RuleFor(c => c.konf_user_password).NotEmpty().WithMessage(msgError1).Length(1, 32).WithMessage(msgError2);
		}
	}
}
