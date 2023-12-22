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

using log4net;
using SparkPOS.Repository.Api;
using SparkPOS.Repository.Api.Report;
using SparkPOS.Repository.Service.Report;
 
namespace SparkPOS.Repository.Service
{    
    public class UnitOfWork : IUnitOfWork
    {
        private IDapperContext _context;
        private ILog _log;
        private bool _isUseWebAPI;
        private string _baseUrl = string.Empty;

        private IDatabaseVersionRepository _databaseversionRepository;
        private ICardRepository _kartuRepository;
        private IReasonAdjustmentStockRepository _alasanpenyesuaianstockRepository;
        private ITitlesRepository _titlesRepository;
        private ITypeExpenseRepository _typeexpenseRepository;
        private ICategoryRepository _golonganRepository; 
        private ITaxRepository _taxRepository;
        private ISettingApplicationRepository _settingApplicationRepository;
        private IProductRepository _produkRepository;
        private IPriceWholesaleRepository _hargaWholesaleRepository;
        private ICustomerRepository _customerRepository;
        private ISupplierRepository _supplierRepository;
        private IDropshipperRepository _dropshipperRepository;
        private IEmployeeRepository _employeeRepository;
        private IPurchaseProductRepository _beliprodukRepository;
        private IReturnPurchaseProductRepository _returnbeliprodukRepository;
        private ISellingProductRepository _jualprodukRepository;
        private ISellingQuotationRepository _jualQuotationRepository; 
        private ISellingDeliveryNotesRepository _jualDeliveryNotesRepository;
        private IDebtPaymentProductRepository _DebtPaymentprodukRepository;
        private IPaymentCreditProductRepository _paymentcreditprodukRepository;
        private IReturnSellingProductRepository _returnjualprodukRepository;
        private ILog4NetRepository _log4NetRepository;
        private IAdjustmentStockRepository _penyesuaianstockRepository;
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private IRolePrivilegeRepository _roleprivilegeRepository;
        private IMenuRepository _menuRepository;
        private IItemMenuRepository _itemmenuRepository;
        private IProfilRepository _profilRepository;
        private ICashierMachineRepository _mesinRepository;
        private IExpenseCostRepository _expensecostRepository;
        private ILoanRepository _kasbonRepository;
        private IPaymentLoanRepository _paymentkasbonRepository;
        private ISalaryEmployeeRepository _gajiemployeeRepository;
        private IPrintInvoiceRepository _printInvoiceRepository;
        private IPrintQuotationRepository _printQuotationRepository; 
        private IPrintDeliveryNotesRepository _printDeliveryNotesRepository;
        private IPrintInvoiceRepository _printInvoiceSampleRepository;
        private IPrintQuotationRepository _printQuotationSampleRepository;

        private IReportPurchaseProductRepository _reportPurchaseProductRepository;
        private IReportDebtPurchaseProductRepository _reportDebtPurchaseProductRepository;
        private IReportDebtPaymentPurchaseProductRepository _reportDebtPaymentPurchaseProductRepository;
        private IReportCardDebtRepository _reportCardDebtRepository;
        private IReportReturnPurchaseProductRepository _reportReturnPurchaseProductRepository;

        private IReportSellingProductRepository _reportSellingProductRepository;
        private IReportSellingQuotationRepository _reportSellingQuotationRepository;
        private IReportCreditSellingProductRepository _reportCreditSellingProductRepository;
        private IReportPaymentCreditSellingProductRepository _reportPaymentCreditSellingProductRepository;
        private IReportCardCreditRepository _reportCardCreditRepository;
        private IReportReturnSellingProductRepository _reportReturnSellingProductRepository;
        private IReportCashierMachineRepository _reportCashierMachineRepository;

        private IReportStockProductRepository _reportStockProductRepository;
        private IReportCardStockRepository _reportCardStockRepository;
        private IReportExpenseCostRepository _reportExpenseCostRepository;
        private IReportLoanRepository _reportLoanRepository;
        private IReportSalaryEmployeeRepository _reportSalaryEmployeeRepository;
        private IReportRevenueExpenseRepository _reportRevenueExpenseRepository;
        private IReportLossProfitRepository _reportLossProfitRepository;

        private IHeaderInvoiceRepository _headerInvoiceRepository;
        private ILabelInvoiceRepository _labelInvoiceRepository;
        private IRegencyShippingCostsByRajaRepository _regencyRepository;
        private IAreaRepository _regionRepository;

        private IFooterInvoiceMiniPosRepository _footernotaminiposRepository;
        private IHeaderInvoiceMiniPosRepository _headernotaminiposRepository;

        public UnitOfWork(IDapperContext context, ILog log)
        {
            this._context = context;
            this._log = log;
        }

        public UnitOfWork(bool isUseWebAPI, string baseUrl, ILog log)
        {
            this._isUseWebAPI = isUseWebAPI;
            this._baseUrl = baseUrl;
            this._log = log;            
        }

        public IDatabaseVersionRepository DatabaseVersionRepository
        {
            get { return _databaseversionRepository ?? (_databaseversionRepository = new DatabaseVersionRepository(_context, _log)); }
        }

        public ICardRepository CardRepository
        {
            get 
            {
                return _kartuRepository ?? (_kartuRepository = _isUseWebAPI ? (ICardRepository)new CardWebAPIRepository(_baseUrl, _log) : new CardRepository(_context, _log));
            }
        }

        public IReasonAdjustmentStockRepository ReasonAdjustmentStockRepository
        {
            get 
            {
                return _alasanpenyesuaianstockRepository ?? (_alasanpenyesuaianstockRepository = _isUseWebAPI ? (IReasonAdjustmentStockRepository)new ReasonAdjustmentStockWebAPIRepository(_baseUrl, _log) : new ReasonAdjustmentStockRepository(_context, _log));
            }
        }

        public ITitlesRepository TitlesRepository
        {
            get
            {
                return _titlesRepository ?? (_titlesRepository = _isUseWebAPI ? (ITitlesRepository)new TitlesWebAPIRepository(_baseUrl, _log) : new TitlesRepository(_context, _log));
            }
        }

        public ITypeExpenseRepository TypeExpenseRepository
        {
            get 
            { 
                return _typeexpenseRepository ?? (_typeexpenseRepository = _isUseWebAPI ? (ITypeExpenseRepository)new TypeExpenseWebAPIRepository(_baseUrl, _log) : new TypeExpenseRepository(_context, _log));
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get 
            {
                return _golonganRepository ?? (_golonganRepository = _isUseWebAPI ? (ICategoryRepository)new CategoryWebAPIRepository(_baseUrl, _log) : new CategoryRepository(_context, _log));
            }
        }
        
        public ITaxRepository TaxRepository
        {
            get 
            {
                return _taxRepository ?? (_taxRepository = _isUseWebAPI ? (ITaxRepository)new TaxWebAPIRepository(_baseUrl, _log) : new TaxRepository(_context, _log));
            }
        }

        public ISettingApplicationRepository SettingApplicationRepository
        {
            get { return _settingApplicationRepository ?? (_settingApplicationRepository = new SettingApplicationRepository(_context, _log)); }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                return _produkRepository ?? (_produkRepository = _isUseWebAPI ? (IProductRepository)new ProductWebAPIRepository(_baseUrl, _log) : new ProductRepository(_context, _log));
            }
        }

        public IPriceWholesaleRepository PriceWholesaleRepository
        {
            get { return _hargaWholesaleRepository ?? (_hargaWholesaleRepository = new PriceWholesaleRepository(_context, _log)); }
        }

        public ICustomerRepository CustomerRepository
        {
            get
            {
                return _customerRepository ?? (_customerRepository = _isUseWebAPI ? (ICustomerRepository)new CustomerWebAPIRepository(_baseUrl, _log) : new CustomerRepository(_context, _log));
            }
        }

        public ISupplierRepository SupplierRepository
        {
            get
            {
                return _supplierRepository ?? (_supplierRepository = _isUseWebAPI ? (ISupplierRepository)new SupplierWebAPIRepository(_baseUrl, _log) : new SupplierRepository(_context, _log));
            }
        }

        public IDropshipperRepository DropshipperRepository
        {
            get 
            { 
                return _dropshipperRepository ?? (_dropshipperRepository = _isUseWebAPI ? (IDropshipperRepository)new DropshipperWebAPIRepository(_baseUrl, _log) : new DropshipperRepository(_context, _log));
            }
        }

        public IEmployeeRepository EmployeeRepository
        {
            get
            {
                return _employeeRepository ?? (_employeeRepository = _isUseWebAPI ? (IEmployeeRepository)new EmployeeWebAPIRepository(_baseUrl, _log) : new EmployeeRepository(_context, _log));
            }
        }

        public IPurchaseProductRepository PurchaseProductRepository
        {
            get
            {
                return _beliprodukRepository ?? (_beliprodukRepository = _isUseWebAPI ? (IPurchaseProductRepository)new PurchaseProductWebAPIRepository(_baseUrl, _log) : new PurchaseProductRepository(_context, _log));
            }
        }

        public IReturnPurchaseProductRepository ReturnPurchaseProductRepository
        {
            get { return _returnbeliprodukRepository ?? (_returnbeliprodukRepository = new ReturnPurchaseProductRepository(_context, _log)); }
        }

        public ISellingProductRepository SellingProductRepository
        {
            get
            {
                return _jualprodukRepository ?? (_jualprodukRepository = _isUseWebAPI ? (ISellingProductRepository)new SellingProductWebAPIRepository(_baseUrl, _log) : new SellingProductRepository(_context, _log));
            }
        }
        public ISellingQuotationRepository SellingQuotationRepository
        {
            get
            {
                return _jualQuotationRepository ?? (_jualQuotationRepository = _isUseWebAPI ? (ISellingQuotationRepository)new SellingProductWebAPIRepository(_baseUrl, _log) : new SellingQuotationRepository(_context, _log));
            }
        }   
        
        public ISellingDeliveryNotesRepository SellingDeliveryNotesRepository
        {
            get
            {
                return _jualDeliveryNotesRepository ?? (_jualDeliveryNotesRepository = _isUseWebAPI ? (ISellingDeliveryNotesRepository)new SellingProductWebAPIRepository(_baseUrl, _log) : new SellingDeliveryNotesRepository(_context, _log));
            }
        }

        public IDebtPaymentProductRepository DebtPaymentProductRepository
        {
            get
            {
                return _DebtPaymentprodukRepository ?? (_DebtPaymentprodukRepository = _isUseWebAPI ? (IDebtPaymentProductRepository)new DebtPaymentProductWebAPIRepository(_baseUrl, _log) : new DebtPaymentProductRepository(_context, _log));
            }
        }

        public IPaymentCreditProductRepository PaymentCreditProductRepository
        {
            get
            {
                return _paymentcreditprodukRepository ?? (_paymentcreditprodukRepository = _isUseWebAPI ? (IPaymentCreditProductRepository)new PaymentCreditProductWebAPIRepository(_baseUrl, _log) : new PaymentCreditProductRepository(_context, _log));
            }
        }

        public IReturnSellingProductRepository ReturnSellingProductRepository
        {
            get { return _returnjualprodukRepository ?? (_returnjualprodukRepository = new ReturnSellingProductRepository(_context, _log)); }
        }

        public ILog4NetRepository Log4NetRepository
        {
            get { return _log4NetRepository ?? (_log4NetRepository = new Log4NetRepository(_context)); }
        }

        public IAdjustmentStockRepository AdjustmentStockRepository
        {
            get
            {
                return _penyesuaianstockRepository ?? (_penyesuaianstockRepository = _isUseWebAPI ? (IAdjustmentStockRepository)new AdjustmentStockWebAPIRepository(_baseUrl, _log) : new AdjustmentStockRepository(_context, _log));
            }
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository ?? (_userRepository = new UserRepository(_context, _log)); }
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository ?? (_roleRepository = new RoleRepository(_context, _log)); }
        }

        public IRolePrivilegeRepository RolePrivilegeRepository
        {
            get { return _roleprivilegeRepository ?? (_roleprivilegeRepository = new RolePrivilegeRepository(_context, _log)); }
        }

        public IMenuRepository MenuRepository
        {
            get { return _menuRepository ?? (_menuRepository = new MenuRepository(_context, _log)); }
        }

        public IItemMenuRepository ItemMenuRepository
        {
            get { return _itemmenuRepository ?? (_itemmenuRepository = new ItemMenuRepository(_context, _log)); }
        }

        public IProfilRepository ProfilRepository
        {
            get { return _profilRepository ?? (_profilRepository = new ProfilRepository(_context, _log)); }
        }

        public ICashierMachineRepository MachineRepository
        {
            get { return _mesinRepository ?? (_mesinRepository = new CashierMachineRepository(_context, _log)); }
        }

        public IReportPurchaseProductRepository ReportPurchaseProductRepository
        {
            get { return _reportPurchaseProductRepository ?? (_reportPurchaseProductRepository = new ReportPurchaseProductRepository(_context, _log)); }
        }

        public IReportDebtPurchaseProductRepository ReportDebtPurchaseProductRepository
        {
            get { return _reportDebtPurchaseProductRepository ?? (_reportDebtPurchaseProductRepository = new ReportDebtPurchaseProductRepository(_context, _log)); }
        }

        public IReportDebtPaymentPurchaseProductRepository ReportDebtPaymentPurchaseProductRepository
        {
            get { return _reportDebtPaymentPurchaseProductRepository ?? (_reportDebtPaymentPurchaseProductRepository = new ReportDebtPaymentPurchaseProductRepository(_context, _log)); }
        }

        public IReportCardDebtRepository ReportCardDebtRepository
        {
            get { return _reportCardDebtRepository ?? (_reportCardDebtRepository = new ReportCardDebtRepository(_context, _log)); }
        }

        public IReportReturnPurchaseProductRepository ReportReturnPurchaseProductRepository
        {
            get { return _reportReturnPurchaseProductRepository ?? (_reportReturnPurchaseProductRepository = new ReportReturnPurchaseProductRepository(_context, _log)); }
        }

        public IReportSellingProductRepository ReportSellingProductRepository
        {
            get { return _reportSellingProductRepository ?? (_reportSellingProductRepository = new ReportSellingProductRepository(_context, _log)); }
        }
        public IReportSellingQuotationRepository ReportSellingQuotationRepository
        {
            get { return _reportSellingQuotationRepository ?? (_reportSellingQuotationRepository = new ReportSellingQuotationRepository(_context, _log)); }
        }

        public IReportCreditSellingProductRepository ReportCreditSellingProductRepository
        {
            get { return _reportCreditSellingProductRepository ?? (_reportCreditSellingProductRepository = new ReportCreditSellingProductRepository(_context, _log)); }
        }

        public IReportPaymentCreditSellingProductRepository ReportPaymentCreditSellingProductRepository
        {
            get { return _reportPaymentCreditSellingProductRepository ?? (_reportPaymentCreditSellingProductRepository = new ReportPaymentCreditSellingProductRepository(_context, _log)); }
        }

        public IReportCardCreditRepository ReportCardCreditRepository
        {
            get { return _reportCardCreditRepository ?? (_reportCardCreditRepository = new ReportCardCreditRepository(_context, _log)); }
        }

        public IReportReturnSellingProductRepository ReportReturnSellingProductRepository
        {
            get { return _reportReturnSellingProductRepository ?? (_reportReturnSellingProductRepository = new ReportReturnSellingProductRepository(_context, _log)); }
        }

        public IReportCashierMachineRepository ReportCashierMachineRepository
        {
            get { return _reportCashierMachineRepository ?? (_reportCashierMachineRepository = new ReportCashierMachineRepository(_context, _log)); }
        }

        public IReportStockProductRepository ReportStockProductRepository
        {
            get { return _reportStockProductRepository ?? (_reportStockProductRepository = new ReportStockProductRepository(_context, _log)); }
        }

        public IReportCardStockRepository ReportCardStockRepository
        {
            get { return _reportCardStockRepository ?? (_reportCardStockRepository = new ReportCardStockRepository(_context, _log)); }
        }

        public IExpenseCostRepository ExpenseCostRepository
        {
            get
            {
                return _expensecostRepository ?? (_expensecostRepository = _isUseWebAPI ? (IExpenseCostRepository)new ExpenseCostWebAPIRepository(_baseUrl, _log) : new ExpenseCostRepository(_context, _log));
            }
        }

        public IReportExpenseCostRepository ReportExpenseCostRepository
        {
            get { return _reportExpenseCostRepository ?? (_reportExpenseCostRepository = new ReportExpenseCostRepository(_context, _log)); }
        }

        public IReportLoanRepository ReportLoanRepository
        {
            get { return _reportLoanRepository ?? (_reportLoanRepository = new ReportLoanRepository(_context, _log)); }
        }

        public IReportSalaryEmployeeRepository ReportSalaryEmployeeRepository
        {
            get { return _reportSalaryEmployeeRepository ?? (_reportSalaryEmployeeRepository = new ReportSalaryEmployeeRepository(_context, _log)); }
        }

        public IReportRevenueExpenseRepository ReportRevenueExpenseRepository
        {
            get { return _reportRevenueExpenseRepository ?? (_reportRevenueExpenseRepository = new ReportRevenueExpenseRepository(_context, _log)); }
        }

        public IReportLossProfitRepository ReportLossProfitRepository
        {
            get { return _reportLossProfitRepository ?? (_reportLossProfitRepository = new ReportLossProfitRepository(_context, _log)); }
        }

        public ILoanRepository LoanRepository
        {
            get
            {
                return _kasbonRepository ?? (_kasbonRepository = _isUseWebAPI ? (ILoanRepository)new LoanWebAPIRepository(_baseUrl, _log) : new LoanRepository(_context, _log));
            }
        }

        public IPaymentLoanRepository PaymentLoanRepository
        {
            get
            {
                return _paymentkasbonRepository ?? (_paymentkasbonRepository = _isUseWebAPI ? (IPaymentLoanRepository)new PaymentLoanWebAPIRepository(_baseUrl, _log) : new PaymentLoanRepository(_context, _log));
            }
        }

        public ISalaryEmployeeRepository SalaryEmployeeRepository
        {
            get
            {
                return _gajiemployeeRepository ?? (_gajiemployeeRepository = _isUseWebAPI ? (ISalaryEmployeeRepository)new SalaryEmployeeWebAPIRepository(_baseUrl, _log) : new SalaryEmployeeRepository(_context, _log));
            }
        }

        public IPrintInvoiceRepository PrintInvoiceSampleRepository
        {
            get { return _printInvoiceSampleRepository ?? (_printInvoiceSampleRepository = new PrintInvoiceSampleRepository()); }
        } 
        
        public IPrintQuotationRepository PrintQuotationSampleRepository
        {
            get { return PrintQuotationSampleRepository ?? (_printQuotationSampleRepository = new PrintQuotationSampleRepository()); }
        }

        public IPrintInvoiceRepository PrintInvoiceRepository
        {
            get { return _printInvoiceRepository ?? (_printInvoiceRepository = new PrintInvoiceRepository(_context, _log)); }
        } 
        public IPrintQuotationRepository PrintQuotationRepository
        {
            get { return _printQuotationRepository ?? (_printQuotationRepository = new PrintQuotationRepository(_context, _log)); }
        }   
        
        public IPrintDeliveryNotesRepository PrintDeliveryNotesRepository
        {
            get { return _printDeliveryNotesRepository ?? (_printDeliveryNotesRepository = new PrintDeliveryNotesRepository(_context, _log)); }
        }

        public IHeaderInvoiceRepository HeaderInvoiceRepository
        {
            get { return _headerInvoiceRepository ?? (_headerInvoiceRepository = new HeaderInvoiceRepository(_context, _log)); }
        }

        public ILabelInvoiceRepository LabelInvoiceRepository
        {
            get { return _labelInvoiceRepository ?? (_labelInvoiceRepository = new LabelInvoiceRepository(_context, _log)); }
        }

        public IRegencyShippingCostsByRajaRepository RegencyRepository
        {
            get { return _regencyRepository ?? (_regencyRepository = new RegencyShippingCostsByRajaRepository(_context, _log)); }
        }

        public IAreaRepository AreaRepository
        {
            get { return _regionRepository ?? (_regionRepository = new AreaRepository(_context, _log)); }
        }

        public IFooterInvoiceMiniPosRepository FooterInvoiceMiniPosRepository
        {
            get { return _footernotaminiposRepository ?? (_footernotaminiposRepository = new FooterInvoiceMiniPosRepository(_context, _log)); }
        }

        public IHeaderInvoiceMiniPosRepository HeaderInvoiceMiniPosRepository
        {
            get { return _headernotaminiposRepository ?? (_headernotaminiposRepository = new HeaderInvoiceMiniPosRepository(_context, _log)); }
        }

        //public ISellingQuotationRepository SellingQuotationRepository => throw new NotImplementedException();
    }
}     
