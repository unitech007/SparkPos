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
 
using SparkPOS.Repository.Api.Report;

namespace SparkPOS.Repository.Api
{    
    public interface IUnitOfWork
    {
        IDatabaseVersionRepository DatabaseVersionRepository { get; }
        ITypeExpenseRepository TypeExpenseRepository { get; }

        ICardRepository CardRepository { get; }
        ICategoryRepository CategoryRepository { get; } 
        ITaxRepository TaxRepository { get; }
        ISettingApplicationRepository SettingApplicationRepository { get; }
        IProductRepository ProductRepository { get; }
        IPriceWholesaleRepository PriceWholesaleRepository { get; }
        IReasonAdjustmentStockRepository ReasonAdjustmentStockRepository { get; }
        IAdjustmentStockRepository AdjustmentStockRepository { get; }

        ICustomerRepository CustomerRepository { get; }
        ISupplierRepository SupplierRepository { get; }
        IDropshipperRepository DropshipperRepository { get; }

        ITitlesRepository TitlesRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        
        IPurchaseProductRepository PurchaseProductRepository { get; }
        IDebtPaymentProductRepository DebtPaymentProductRepository { get; }
        IReturnPurchaseProductRepository ReturnPurchaseProductRepository { get; }              
  
        ISellingProductRepository SellingProductRepository { get; }
        ISellingQuotationRepository SellingQuotationRepository { get; } 
        
        ISellingDeliveryNotesRepository SellingDeliveryNotesRepository { get; }
        IPaymentCreditProductRepository PaymentCreditProductRepository { get; }
        IReturnSellingProductRepository ReturnSellingProductRepository { get; }

        IExpenseCostRepository ExpenseCostRepository { get; }
        ILoanRepository LoanRepository { get; }
        IPaymentLoanRepository PaymentLoanRepository { get; }
        ISalaryEmployeeRepository SalaryEmployeeRepository { get; }

        IPrintInvoiceRepository PrintInvoiceRepository { get; }
        IPrintInvoiceRepository PrintInvoiceSampleRepository { get; }
        IPrintQuotationRepository PrintQuotationSampleRepository { get; }
        IPrintQuotationRepository PrintQuotationRepository { get; }
        IPrintDeliveryNotesRepository PrintDeliveryNotesRepository { get; }

        ILog4NetRepository Log4NetRepository { get; }
        
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IRolePrivilegeRepository RolePrivilegeRepository { get; }
        IMenuRepository MenuRepository { get; }
        IItemMenuRepository ItemMenuRepository { get; }
        IHeaderInvoiceRepository HeaderInvoiceRepository { get; }
        ILabelInvoiceRepository LabelInvoiceRepository { get; }

        IProfilRepository ProfilRepository { get; }

        ICashierMachineRepository MachineRepository { get; }

        IReportPurchaseProductRepository ReportPurchaseProductRepository { get; }
        IReportDebtPurchaseProductRepository ReportDebtPurchaseProductRepository { get; }
        IReportDebtPaymentPurchaseProductRepository ReportDebtPaymentPurchaseProductRepository { get; }
        IReportCardDebtRepository ReportCardDebtRepository { get; }
        IReportReturnPurchaseProductRepository ReportReturnPurchaseProductRepository { get; }

        IReportSellingProductRepository ReportSellingProductRepository { get; }
        IReportCreditSellingProductRepository ReportCreditSellingProductRepository { get; }
        IReportPaymentCreditSellingProductRepository ReportPaymentCreditSellingProductRepository { get; }
        IReportCardCreditRepository ReportCardCreditRepository { get; }
        IReportReturnSellingProductRepository ReportReturnSellingProductRepository { get; }
        IReportCashierMachineRepository ReportCashierMachineRepository { get; }

        IReportStockProductRepository ReportStockProductRepository { get; }
        IReportCardStockRepository ReportCardStockRepository { get; }
        IReportExpenseCostRepository ReportExpenseCostRepository { get; }
        IReportLoanRepository ReportLoanRepository { get; }
        IReportSalaryEmployeeRepository ReportSalaryEmployeeRepository { get; }
        IReportRevenueExpenseRepository ReportRevenueExpenseRepository { get; }
        IReportLossProfitRepository ReportLossProfitRepository { get; }

        IRegencyShippingCostsByRajaRepository RegencyRepository { get; }
        IAreaRepository AreaRepository { get; }
        IFooterInvoiceMiniPosRepository FooterInvoiceMiniPosRepository { get; }
        IHeaderInvoiceMiniPosRepository HeaderInvoiceMiniPosRepository { get; }        
    }
}     
