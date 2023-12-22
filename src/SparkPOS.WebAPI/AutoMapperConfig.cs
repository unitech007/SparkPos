using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using AutoMapper;
using SparkPOS.Model;
using SparkPOS.WebAPI.Models.DTO;

namespace SparkPOS.WebAPI
{
    public class AutoMapperConfig
    {
        public static void Initialize()
        {
            Mapper.Initialize((config) =>
            {
                // Reference
                config.CreateMap<CardDTO, Card>();
                config.CreateMap<ReasonAdjustmentStockDTO, ReasonAdjustmentStock>();
                config.CreateMap<AdjustmentStockDTO, AdjustmentStock>();
                config.CreateMap<DropshipperDTO, Dropshipper>();
                config.CreateMap<CategoryDTO, Category>();
                config.CreateMap<ProductDTO, Product>();
                config.CreateMap<PriceWholesaleDTO, PriceWholesale>();
                config.CreateMap<TypeExpenseDTO, TypeExpense>();
                config.CreateMap<TitlesDTO, job_titles>();
                config.CreateMap<ProvinsiDTO, Provinsi>();
                config.CreateMap<RegencyDTO, Regency>();
                config.CreateMap<subdistrictDTO, subdistrict>();
                config.CreateMap<CustomerDTO, Customer>();
                config.CreateMap<SupplierDTO, Supplier>();                
                config.CreateMap<EmployeeDTO, Employee>();

                // expense
                config.CreateMap<ExpenseCostDTO, ExpenseCost>();
                config.CreateMap<ItemExpenseCostDTO, ItemExpenseCost>();
                config.CreateMap<LoanDTO, loan>();
                config.CreateMap<PaymentLoanDTO, PaymentLoan>();
                config.CreateMap<SalaryEmployeeDTO, SalaryEmployee>();

                // transactions Purchase
                config.CreateMap<PurchaseProductDTO, PurchaseProduct>();
                config.CreateMap<ItemPurchaseProductDTO, ItemPurchaseProduct>();
                config.CreateMap<ReturnPurchaseProductDTO, ReturnPurchaseProduct>();

                // transactions Dept Payment
                config.CreateMap<DebtPaymentProductDTO, DebtPaymentProduct>();
                config.CreateMap<ItemDebtPaymentProductDTO, ItemDebtPaymentProduct>();

                // transactions sales
                config.CreateMap<SellingProductDTO, SellingProduct>();
                config.CreateMap<ItemSellingProductDTO, ItemSellingProduct>();
                config.CreateMap<ReturnSellingProductDTO, ReturnSellingProduct>();

                // transactions payment credit
                config.CreateMap<PaymentCreditProductDTO, PaymentCreditProduct>();
                config.CreateMap<ItemPaymentCreditProductDTO, ItemPaymentCreditProduct>();

                // settings
                config.CreateMap<RoleDTO, Role>();
                config.CreateMap<UserDTO, User>();
            });
        }
    }
}
