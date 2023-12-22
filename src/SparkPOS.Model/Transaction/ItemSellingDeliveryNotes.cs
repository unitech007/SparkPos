
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using Dapper.Contrib.Extensions;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;



namespace SparkPOS.Model.Transaction
{

    [Table("t_delivery_items")]
    public class ItemSellingDeliveryNotes
    {
        public ItemSellingDeliveryNotes()
        {
            entity_state = EntityState.Added;
        }



       
        [ExplicitKey]
        [Display(Name = "delivery_item_id")]
        public string delivery_item_id { get; set; }

        [Display(Name = "delivery_id")]
        public string delivery_id { get; set; }

        [JsonIgnore]
        [Write(false)]
        public SellingQuotation SellingQuotation { get; set; }

        [Display(Name = "user_id")]
        public string user_id { get; set; }

        [JsonIgnore]
        [Write(false)]
        public User User { get; set; }

        [Display(Name = "product_id")]
        public string product_id { get; set; }

        [Write(false)]
        public Product Product { get; set; }

        [Display(Name = "Description Optional")]
        public string description { get; set; }

        [Display(Name = "Buying Price")]
        public double purchase_price { get; set; }

        [Display(Name = "price Selling")]
        public double selling_price { get; set; }

        [Display(Name = "return_quantity")]
        public double return_quantity { get; set; }

        //[Write(false)]
        //[Display(Name = "Old quantity")]
        //public double old_jumlah { get; set; }

        [Display(Name = "quantity")]
        public double quantity { get; set; }

        [Display(Name = "discount")]
        public double discount { get; set; }

        [JsonIgnore]
        [Write(false)]
        [Display(Name = "system_date")]
        public Nullable<DateTime> system_date { get; set; }

        //[Write(false)]
        //[Display(Name = "return_quantity")]
        //public double return_quantity { get; set; }

        //[JsonIgnore]
        [Computed]
        public double diskon_rupiah
        {
            get { return discount / 100 * selling_price; }
        }

        //[JsonIgnore]
        [Computed]
        public double harga_setelah_diskon
        {
            get { return selling_price - diskon_rupiah; }
        }

        //[JsonIgnore]
        //[Computed]
        //public double sub_total
        //{
        //    get { return (quantity - return_quantity) * harga_setelah_diskon; }
        //}

        [Write(false)]
        public EntityState entity_state { get; set; }
    }

    public class ItemSellingDeliveryNotesValidator : AbstractValidator<ItemSellingDeliveryNotes>
    {
        public ItemSellingDeliveryNotesValidator()
        {
            CascadeMode = FluentValidation.CascadeMode.StopOnFirstFailure;

            var msgError1 = "'{PropertyName}' should not empty !";
            var msgError2 = "Inputan '{PropertyName}' maksimal {MaxLength} karakter !";

            RuleFor(c => c.delivery_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.user_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.product_id).NotEmpty().WithMessage(msgError1).Length(1, 36).WithMessage(msgError2);
            RuleFor(c => c.description).Length(0, 100).WithMessage(msgError2);
        }
    }
}
