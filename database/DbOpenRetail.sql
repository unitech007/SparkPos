--
-- PostgreSQL database dump
--

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE DATABASE openretail;


CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET search_path = public, pg_catalog;

--
-- Name: t_address; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_address AS character varying(100);


ALTER DOMAIN t_address OWNER TO postgres;

--
-- Name: t_address_panjang; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_address_panjang AS character varying(250);


ALTER DOMAIN t_address_panjang OWNER TO postgres;

--
-- Name: t_bool; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_bool AS boolean DEFAULT true;


ALTER DOMAIN t_bool OWNER TO postgres;

--
-- Name: t_diskon; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_diskon AS numeric(5,2) DEFAULT 0.00;


ALTER DOMAIN t_diskon OWNER TO postgres;

--
-- Name: t_guid; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_guid AS character(36);


ALTER DOMAIN t_guid OWNER TO postgres;

--
-- Name: t_price; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_price AS numeric(15,2) DEFAULT 0.00;


ALTER DOMAIN t_price OWNER TO postgres;

--
-- Name: t_quantity; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_quantity AS numeric(10,2) DEFAULT 0.00;


ALTER DOMAIN t_quantity OWNER TO postgres;

--
-- Name: t_description; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_description AS character varying(100);


ALTER DOMAIN t_description OWNER TO postgres;

--
-- Name: t_postal_code; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_postal_code AS character varying(6);


ALTER DOMAIN t_postal_code OWNER TO postgres;

--
-- Name: t_product_code; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_product_code AS character varying(15);


ALTER DOMAIN t_product_code OWNER TO postgres;

--
-- Name: t_name; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_name AS character varying(50);


ALTER DOMAIN t_name OWNER TO postgres;

--
-- Name: t_name_panjang; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_name_panjang AS character varying(300);


ALTER DOMAIN t_name_panjang OWNER TO postgres;

--
-- Name: t_invoice; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_invoice AS character varying(20);


ALTER DOMAIN t_invoice OWNER TO postgres;

--
-- Name: t_password; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_password AS character(32);


ALTER DOMAIN t_password OWNER TO postgres;

--
-- Name: t_unit; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_unit AS character varying(20);


ALTER DOMAIN t_unit OWNER TO postgres;

--
-- Name: t_phone; Type: DOMAIN; Schema: public; Owner: postgres
--

CREATE DOMAIN t_phone AS character varying(20);


ALTER DOMAIN t_phone OWNER TO postgres;

--
-- Name: f_delete_payable_product_header(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_delete_payable_product_header() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE var_pembayaran_hutang_product_id t_guid;
DECLARE var_row_count integer;

BEGIN		    
    var_pembayaran_hutang_product_id := OLD.pembayaran_hutang_product_id;
	
    var_row_count := (select count(*) from t_debt_payment_item
                      where pembayaran_hutang_product_id = var_pembayaran_hutang_product_id);
                      
    IF var_row_count IS NULL THEN
        var_row_count := 0;  
    END IF;
	
	IF (var_row_count = 0) THEN
    	DELETE FROM t_product_payable_payment WHERE pembayaran_hutang_product_id = var_pembayaran_hutang_product_id;
    END IF;
    
  	RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_delete_payable_product_header() OWNER TO postgres;

--
-- Name: f_delete_receivable_product_header(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_delete_receivable_product_header() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE var_pay_sale_id t_guid;
DECLARE var_row_count integer;

BEGIN		    
    var_pay_sale_id := OLD.pay_sale_id;
	
    var_row_count := (select count(*) from t_item_pembayaran_piutang_produk
                      where pay_sale_id = var_pay_sale_id);
                      
    IF var_row_count IS NULL THEN
        var_row_count := 0;  
    END IF;
	
	IF (var_row_count = 0) THEN
    	DELETE FROM t_product_receivable_paymentWHERE pay_sale_id = var_pay_sale_id;
    END IF;
    
  	RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_delete_receivable_product_header() OWNER TO postgres;

--
-- Name: f_reduce_product_stock(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_reduce_product_stock() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_product_id 				t_guid;
    
	var_stock_sekarang 			t_quantity; -- stock etalase	
    var_warehouse_stock_sekarang 	t_quantity; -- stock gudang
    
    var_quantity_lama 		t_quantity;
	var_quantity_baru 		t_quantity;
    
    var_return_quantity_lama 	t_quantity;
    var_return_quantity_baru 	t_quantity;

	var_price 				t_price;
    
    is_retur				t_bool;
    is_update_selling_price	t_bool;
    
BEGIN		
	is_retur := FALSE;
                
    IF TG_OP = 'INSERT' THEN
    	var_product_id := NEW.product_id;
        var_quantity_baru = NEW.quantity;        
        var_price = NEW.selling_price;

	ELSIF TG_OP = 'UPDATE' THEN      
    	var_product_id := NEW.product_id;
        var_quantity_baru = NEW.quantity;
        var_quantity_lama = OLD.quantity;
        var_price = NEW.selling_price;
        
        -- quantity retur
        var_return_quantity_baru = NEW.return_quantity;
        var_return_quantity_lama = OLD.return_quantity;        
    ELSE
    	var_product_id := OLD.product_id;
        var_quantity_lama = OLD.quantity;
        var_price = OLD.selling_price;
    END IF;        
            
    SELECT stock, warehouse_stock INTO var_stock_sekarang, var_warehouse_stock_sekarang
    FROM m_product WHERE product_id = var_product_id;
    
    IF var_stock_sekarang IS NULL THEN -- stock etalase
        var_stock_sekarang := 0;  
    END IF;
    
    IF var_warehouse_stock_sekarang IS NULL THEN -- stock gudang
        var_warehouse_stock_sekarang := 0;  
    END IF;
        
    IF TG_OP = 'UPDATE' THEN -- pengecekan retur
    	IF var_return_quantity_lama <> var_return_quantity_baru THEN
        	is_retur := TRUE;                    
            
            IF var_return_quantity_lama IS NULL THEN
                var_return_quantity_lama := 0;  
            END IF;
            
            IF var_return_quantity_baru IS NULL THEN
                var_return_quantity_baru := 0;  
            END IF;
                           
            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang - var_return_quantity_lama + var_return_quantity_baru;
			            
            UPDATE m_product SET warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;
        END IF;        
    END IF;
	
    IF (is_retur = FALSE) THEN -- bukan retur    	    	        
        IF TG_OP = 'INSERT' THEN
            var_warehouse_stock_sekarang := var_warehouse_stock_sekarang - var_quantity_baru;
            
            IF (var_warehouse_stock_sekarang < 0 AND var_stock_sekarang > 0) THEN -- stock gudang kurang, sisanya ambil dari stock etalase
            	var_stock_sekarang = var_stock_sekarang - abs(var_warehouse_stock_sekarang);
                
                IF (var_stock_sekarang >= 0) THEN
                	var_warehouse_stock_sekarang := 0; --stock gudang habis
                ELSE
                	var_warehouse_stock_sekarang := var_stock_sekarang;
                    var_stock_sekarang := 0;
                END IF;
            END IF;
                        
            
        ELSIF TG_OP = 'UPDATE' THEN      
            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_lama - var_quantity_baru;
        ELSE
            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_lama;
        END IF;
        
        IF TG_OP = 'INSERT' THEN   
            -- baca setting aplikasi
        	SELECT is_update_selling_price_master_produk INTO is_update_selling_price
            FROM m_application_setting LIMIT 1;
            
            IF is_update_selling_price IS NULL THEN
				is_update_selling_price := TRUE;
            END IF;
    
        	IF (is_update_selling_price = TRUE) THEN -- update price jual di master produk
            	UPDATE m_product SET stock = var_stock_sekarang, warehouse_stock = var_warehouse_stock_sekarang, selling_price = var_price WHERE product_id = var_product_id;
            ELSE          
	            UPDATE m_product SET stock = var_stock_sekarang, warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;        
			END IF;         
        ELSE        
            UPDATE m_product SET stock = var_stock_sekarang, warehouse_stock = var_warehouse_stock_sekarang WHERE product_id = var_product_id;        
        END IF;    
    END IF;	
            
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_reduce_product_stock() OWNER TO postgres;

--
-- Name: f_adjust_product_stock_aiud(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_adjust_product_stock_aiud() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
	var_product_id 						t_guid;
    
    var_stok_etalase					t_quantity;
    var_warehouse_stock						t_quantity;
    
    var_stock_addition_etalase			t_quantity;
    var_penambahan_warehouse_stock			t_quantity;
    
    var_stock_reduction_etalase		t_quantity;
    var_pengurangan_warehouse_stock			t_quantity;
    
    var_stock_addition_etalase_old		t_quantity;
    var_penambahan_warehouse_stock_old		t_quantity;
    
    var_stock_reduction_etalase_old	t_quantity;
    var_pengurangan_warehouse_stock_old		t_quantity;
    
BEGIN    
	IF TG_OP = 'INSERT' THEN
    	var_product_id := NEW.product_id;
        
        var_stock_addition_etalase = NEW.stock_addition;        
		var_penambahan_warehouse_stock = NEW.penambahan_warehouse_stock;        
        
        var_stock_reduction_etalase = NEW.stock_reduction;        
		var_pengurangan_warehouse_stock = NEW.pengurangan_warehouse_stock;        

	ELSIF TG_OP = 'UPDATE' THEN      
    	var_product_id := NEW.product_id;
        
        var_stock_addition_etalase = NEW.stock_addition;        
        var_stock_addition_etalase_old = OLD.stock_addition;        
        
		var_penambahan_warehouse_stock = NEW.penambahan_warehouse_stock;        
        var_penambahan_warehouse_stock_old = OLD.penambahan_warehouse_stock;        
        
        var_stock_reduction_etalase = NEW.stock_reduction;        
        var_stock_reduction_etalase_old = OLD.stock_reduction;        
        
		var_pengurangan_warehouse_stock = NEW.pengurangan_warehouse_stock;        
        var_pengurangan_warehouse_stock_old = OLD.pengurangan_warehouse_stock;        
               
    ELSE
    	var_product_id := OLD.product_id;
        
        var_stock_addition_etalase_old = OLD.stock_addition;                
        var_penambahan_warehouse_stock_old = OLD.penambahan_warehouse_stock;        
        
        var_stock_reduction_etalase_old = OLD.stock_reduction;                
        var_pengurangan_warehouse_stock_old = OLD.pengurangan_warehouse_stock; 
    END IF;
    
    SELECT stok, warehouse_stock INTO var_stok_etalase, var_warehouse_stock
    FROM m_product WHERE product_id = var_product_id;
    
    IF var_stok_etalase IS NULL THEN
        var_stok_etalase := 0;  
    END IF;
    
    IF var_warehouse_stock IS NULL THEN
        var_warehouse_stock := 0;  
    END IF;
    
    IF TG_OP = 'INSERT' THEN		         
        IF (var_stock_addition_etalase > 0 OR var_penambahan_warehouse_stock > 0) THEN
        	var_stok_etalase := var_stok_etalase + var_stock_addition_etalase;
            var_warehouse_stock := var_warehouse_stock + var_penambahan_warehouse_stock;                    	
        END IF;   	        
        
        IF (var_stock_reduction_etalase > 0 OR var_pengurangan_warehouse_stock > 0) THEN
        	var_stok_etalase := var_stok_etalase - var_stock_reduction_etalase;
            var_warehouse_stock := var_warehouse_stock - var_pengurangan_warehouse_stock;                    	
        END IF;        

	ELSIF TG_OP = 'UPDATE' THEN      		      	                     
        IF (var_stock_addition_etalase > 0 OR var_stock_addition_etalase_old > 0 OR var_penambahan_warehouse_stock > 0 OR var_penambahan_warehouse_stock_old > 0) THEN
        	var_stok_etalase := var_stok_etalase - var_stock_addition_etalase_old + var_stock_addition_etalase;
            var_warehouse_stock := var_warehouse_stock - var_penambahan_warehouse_stock_old + var_penambahan_warehouse_stock;                    	
        END IF;
        
        IF (var_stock_reduction_etalase > 0 OR var_stock_reduction_etalase_old > 0 OR var_pengurangan_warehouse_stock > 0 OR var_pengurangan_warehouse_stock_old > 0) THEN
			var_stok_etalase := var_stok_etalase + var_stock_reduction_etalase_old - var_stock_reduction_etalase;
            var_warehouse_stock := var_warehouse_stock + var_pengurangan_warehouse_stock_old - var_pengurangan_warehouse_stock;                    	
        END IF;                        
        
    ELSE    	        
        IF (var_stock_addition_etalase_old > 0 OR var_penambahan_warehouse_stock_old > 0) THEN
        	var_stok_etalase := var_stok_etalase - var_stock_addition_etalase_old;
            var_warehouse_stock := var_warehouse_stock - var_penambahan_warehouse_stock_old;
        END IF;
        
        IF (var_stock_reduction_etalase_old > 0 OR var_pengurangan_warehouse_stock_old > 0) THEN
        	var_stok_etalase := var_stok_etalase + var_stock_reduction_etalase_old;
            var_warehouse_stock := var_warehouse_stock + var_pengurangan_warehouse_stock_old;
        END IF;
    END IF;    
    
    UPDATE m_product SET stok = var_stok_etalase, warehouse_stock = var_warehouse_stock WHERE product_id = var_product_id;
            
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_adjust_product_stock_aiud() OWNER TO postgres;

--
-- Name: f_add_product_stock(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_add_product_stock() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_product_id 				t_guid;
	
    var_stock_awal				t_quantity;
	var_stock_akhir				t_quantity;
    var_stock_sekarang 			t_quantity; -- stock etalase
	var_warehouse_stock_sekarang 	t_quantity; -- stock gudang
    
    var_quantity_lama 			t_quantity;
	var_quantity_baru 			t_quantity;
	
    var_return_quantity_lama 		t_quantity;
    var_return_quantity_baru 		t_quantity;
    
	var_price 					t_price; -- price beli di item beli
    var_hpp_awal				t_price; -- price beli di master produk
    var_hpp_akhir				t_price;
	var_starting_balance				t_price; -- (stock etalasi + stock gudang) * price beli di master produk 
    var_saldo_pembelian			t_price;
    
    is_retur					t_bool;
  
BEGIN		   
	is_retur := FALSE;	
    	     
    IF TG_OP = 'INSERT' THEN
    	var_product_id := NEW.product_id;
        var_quantity_baru = NEW.quantity;
        var_price = NEW.price;
        
	ELSIF TG_OP = 'UPDATE' THEN      
    	var_product_id := NEW.product_id;
        var_quantity_baru = NEW.quantity;
        var_quantity_lama = OLD.quantity;
        var_price = NEW.price;
        
        -- quantity retur
        var_return_quantity_baru = NEW.return_quantity;
        var_return_quantity_lama = OLD.return_quantity;                
    ELSE
    	var_product_id := OLD.product_id;
        var_quantity_baru = 0;
        var_quantity_lama = OLD.quantity;
        var_price = OLD.price;
    END IF;        
    
    SELECT	stock, warehouse_stock, purchase_price INTO var_stock_sekarang, var_warehouse_stock_sekarang, var_hpp_awal
    FROM m_product WHERE product_id = var_product_id;
    
    IF var_stock_sekarang IS NULL THEN
    	var_stock_sekarang := 0;  
	END IF;
    
    IF var_warehouse_stock_sekarang IS NULL THEN
    	var_warehouse_stock_sekarang := 0;  
	END IF;
    
    IF var_hpp_awal IS NULL THEN
    	var_hpp_awal := 0;  
	END IF;        
    
    var_stock_akhir := 1;
    var_stock_awal := var_stock_sekarang + var_warehouse_stock_sekarang;    
    var_starting_balance := var_stock_awal * var_hpp_awal;
    
    IF TG_OP = 'UPDATE' THEN -- pengecekan retur
    	IF var_return_quantity_lama <> var_return_quantity_baru THEN
        	is_retur := TRUE;                    
            
            IF var_return_quantity_lama IS NULL THEN
                var_return_quantity_lama := 0;  
            END IF;
            
            IF var_return_quantity_baru IS NULL THEN
                var_return_quantity_baru := 0;  
            END IF;
			          
            var_saldo_pembelian := ABS(var_return_quantity_lama - var_return_quantity_baru) * var_price;  
            
            var_stock_akhir := (var_stock_awal - ABS(var_return_quantity_lama - var_return_quantity_baru));
            IF (var_stock_akhir = 0) THEN
            	var_stock_akhir := 1;
            END IF;
            
            var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stock_akhir;
                                       
            var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_return_quantity_lama - var_return_quantity_baru;			            
                          			            
            UPDATE m_product SET warehouse_stock = var_warehouse_stock_sekarang, purchase_price = ROUND(var_hpp_akhir, 0) WHERE product_id = var_product_id;
        END IF;        
    END IF;
    
    IF (is_retur = FALSE) THEN -- bukan retur    	        	                                    
      IF TG_OP = 'INSERT' THEN                        
          var_saldo_pembelian := var_quantity_baru * var_price;          
          
          var_stock_akhir := (var_stock_awal + var_quantity_baru);
          
          IF (var_stock_akhir = 0) THEN
              var_stock_akhir := 1;
          END IF;
          
		  var_hpp_akhir := (var_starting_balance + var_saldo_pembelian) / var_stock_akhir;                                         
          
          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang + var_quantity_baru;
          
      ELSIF TG_OP = 'UPDATE' THEN             
          var_saldo_pembelian := ABS(var_quantity_baru - var_quantity_lama) * var_price;
          var_hpp_akhir := var_hpp_awal;
          
      	  IF (var_quantity_baru > var_quantity_lama) THEN      
          	
          	 var_stock_akhir := (var_stock_awal + var_quantity_baru - var_quantity_lama);
             IF (var_stock_akhir = 0) THEN
             	var_stock_akhir := 1;
             END IF;

          	 var_hpp_akhir := (var_starting_balance + var_saldo_pembelian) / var_stock_akhir;                     
             
          ELSEIF (var_quantity_baru < var_quantity_lama) THEN          	 
          	 
          	 var_stock_akhir := (var_stock_awal - ABS(var_quantity_baru - var_quantity_lama));
             IF (var_stock_akhir = 0) THEN
             	var_stock_akhir := 1;
             END IF;
             
          	 var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stock_akhir;                                   
          END IF;          
                                        
          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang - var_quantity_lama + var_quantity_baru;
      ELSE -- DELETE
          var_saldo_pembelian := var_quantity_lama * var_price;
          
          var_stock_akhir := (var_stock_awal - var_quantity_lama);
          IF (var_stock_akhir = 0) THEN
             var_stock_akhir := 1;
          END IF;
             
          var_hpp_akhir := (var_starting_balance - var_saldo_pembelian) / var_stock_akhir;                     
                    
          var_warehouse_stock_sekarang = var_warehouse_stock_sekarang - var_quantity_lama;          
      END IF;          
      
	  UPDATE m_product SET warehouse_stock = var_warehouse_stock_sekarang, purchase_price = ROUND(var_hpp_akhir, 0) WHERE product_id = var_product_id;              
    END IF;    	
            
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_add_product_stock() OWNER TO postgres;
--
-- Name: f_update_purchase_return_quantity(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_purchase_return_quantity() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
	var_purchase_item_id 		t_guid;
    var_return_quantity		t_quantity;
        
BEGIN	
      
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_purchase_item_id := NEW.purchase_item_id;    
        var_return_quantity := NEW.return_quantity;
    ELSE
    	var_purchase_item_id := OLD.purchase_item_id;
        var_return_quantity := 0;
    END IF;
    
    IF var_return_quantity IS NULL THEN
        var_return_quantity := 0;  
    END IF; 
            
    UPDATE t_purchase_order_item SET return_quantity = var_return_quantity 
    WHERE purchase_item_id = var_purchase_item_id;
	
	RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_purchase_return_quantity() OWNER TO postgres;

--
-- Name: f_update_return_quantity_sales(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_return_quantity_sales() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
	var_sale_item_id 		t_guid;
    var_return_quantity		t_quantity;
        
BEGIN	
      
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_sale_item_id := NEW.sale_item_id;    
        var_return_quantity := NEW.return_quantity;
    ELSE
    	var_sale_item_id := OLD.sale_item_id;
        var_return_quantity := 0;
    END IF;
    
    IF var_return_quantity IS NULL THEN
        var_return_quantity := 0;  
    END IF; 
            
    UPDATE t_sales_order_item SET return_quantity = var_return_quantity 
    WHERE sale_item_id = var_sale_item_id;
	
	RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_return_quantity_sales() OWNER TO postgres;

--
-- Name: f_update_purchase_payment(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_purchase_payment() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE var_purchase_id t_guid;
DECLARE var_pelunasan_nota t_price;
  
BEGIN
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_purchase_id := NEW.purchase_id;    
    ELSE
    	var_purchase_id := OLD.purchase_id;
    END IF;
	        
    var_pelunasan_nota := (SELECT SUM(amount) FROM t_debt_payment_item 
    			 	       WHERE purchase_id = var_purchase_id);
	
    IF var_pelunasan_nota IS NULL THEN
    	var_pelunasan_nota := 0;  
	END IF;
    
    UPDATE t_purchase_product SET total_payment = var_pelunasan_nota 
    WHERE purchase_id = var_purchase_id;                        
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_purchase_payment() OWNER TO postgres;

--
-- Name: f_update_sales_payment(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_sales_payment() RETURNS trigger
    LANGUAGE plpgsql
    AS $$

DECLARE var_sale_id t_guid;

DECLARE var_pelunasan_nota t_price;

  

BEGIN

	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN

    	var_sale_id := NEW.sale_id;    

    ELSE

    	var_sale_id := OLD.sale_id;

    END IF;

	        

    var_pelunasan_nota := (SELECT SUM(amount) FROM t_credit_payment_item

    			 	       WHERE sale_id = var_sale_id);	

    IF var_pelunasan_nota IS NULL THEN

    	var_pelunasan_nota := 0;  

	END IF;

    

    UPDATE t_product_sales SET total_payment = var_pelunasan_nota 

    WHERE sale_id = var_sale_id;                        

    

    RETURN NULL;

END;

$$;


ALTER FUNCTION public.f_update_sales_payment() OWNER TO postgres;

--
-- Name: f_update_loan_payment(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_loan_payment() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_loan_id				t_guid;   
    var_gaji_employee_id		t_guid; 
	var_total_settlement_loan 	t_price;

BEGIN
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_loan_id := NEW.loan_id;    
        var_gaji_employee_id := NEW.gaji_employee_id;
    ELSE
    	var_loan_id := OLD.loan_id;
        var_gaji_employee_id := OLD.gaji_employee_id;
    END IF;
	        
    -- pelunasan loan
    var_total_settlement_loan := (SELECT SUM(amount) FROM t_pembayaran_loan 
    							   WHERE loan_id = var_loan_id);	    
	
    IF var_total_settlement_loan IS NULL THEN
    	var_total_settlement_loan := 0;  
	END IF;        
    
    -- loan
    UPDATE t_loan SET total_payment = var_total_settlement_loan 
    WHERE loan_id = var_loan_id;    
    
    -- hitung total pelunasan yang dibayar pake gaji (deductions gaji untuk loan) 
    IF NOT (var_gaji_employee_id IS NULL) THEN
    	-- pelunasan loan
    	var_total_settlement_loan := (SELECT SUM(amount) FROM t_pembayaran_loan 
    							   	   WHERE gaji_employee_id = var_gaji_employee_id);	    
	
	    IF var_total_settlement_loan IS NULL THEN
    		var_total_settlement_loan := 0;  
		END IF;
    	
        UPDATE t_employee_salary SET loan = var_total_settlement_loan 
	    WHERE gaji_employee_id = var_gaji_employee_id;
	END IF;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_loan_payment() OWNER TO postgres;

--
-- Name: f_update_total_purchase(); Type: FUNCTION; Schema: public; Owner: postgres
--
CREATE FUNCTION f_update_total_purchase() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_purchase_id 		t_guid;
    var_return_purchase_id		t_guid;
    var_diskon				t_price;
    var_ppn					t_price;
	var_total_nota 			t_price;
  	var_return_quantity_lama 	t_quantity;
    var_return_quantity_baru 	t_quantity;
    var_date_tempo		DATE;  
    is_retur				t_bool;
    
BEGIN
	is_retur := FALSE;
    
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_purchase_id := NEW.purchase_id;    
    ELSE
    	var_purchase_id := OLD.purchase_id;
    END IF;	 
                       
    var_total_nota := (SELECT SUM((quantity - return_quantity) * (price - (discount / 100 * price))) 
    				   FROM t_purchase_order_item
					   WHERE purchase_id = var_purchase_id);
	
    IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;              
    --
    SELECT tax, discount INTO var_ppn, var_diskon
    FROM t_purchase_product WHERE purchase_id = var_purchase_id;            
    
    IF var_ppn IS NULL THEN
    	var_ppn := 0;  
	END IF;
    
    IF var_diskon IS NULL THEN
    	var_diskon := 0;  
	END IF;           

	IF TG_OP = 'UPDATE' THEN -- pengecekan retur
    	-- quantity retur
        var_return_quantity_baru = NEW.return_quantity;
        var_return_quantity_lama = OLD.return_quantity;        
        
    	IF var_return_quantity_lama <> var_return_quantity_baru THEN
			is_retur := TRUE;
			
            var_return_purchase_id := (SELECT purchase_return_id FROM t_purchase_return WHERE purchase_id = var_purchase_id LIMIT 1);
			var_date_tempo := (SELECT date_tempo FROM t_purchase_product WHERE purchase_id = var_purchase_id);                                    
            
            IF var_date_tempo IS NULL THEN -- nota tunai            	
                UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0), purchase_return_id = var_return_purchase_id 
                WHERE purchase_id = var_purchase_id;                
                
                UPDATE t_debt_payment_item SET amount = ROUND(var_total_nota, 0) - var_diskon + var_ppn
                WHERE purchase_id = var_purchase_id;
            ELSE
            	UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0), purchase_return_id = var_return_purchase_id 
                WHERE purchase_id = var_purchase_id;
            END IF;        
        END IF;        
    END IF;
    
    IF (is_retur = FALSE) THEN -- bukan retur    	
	    UPDATE t_purchase_product SET total_invoice = ROUND(var_total_nota, 0) 
        WHERE purchase_id = var_purchase_id;
    END IF;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_total_purchase() OWNER TO postgres;


--
-- Name: f_update_supplier_total_debt(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_supplier_total_debt() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 	
    var_supplier_id 				t_guid;  
    var_supplier_id_old				t_guid;
    
    var_total_debt_produk			t_price;
  	var_total_settlement_produk		t_price;
    
BEGIN	    
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_supplier_id := NEW.supplier_id;        
    ELSE
    	var_supplier_id := OLD.supplier_id;
        var_supplier_id_old := OLD.supplier_id;
    END IF;
	    	           	                
    -- hitung total hutang dan pelunasan pembelian produk
    SELECT SUM(total_invoice - discount + tax) AS total_debt, SUM(total_payment) AS total_payment
    INTO var_total_debt_produk, var_total_settlement_produk
    FROM t_purchase_product 
    WHERE date_tempo IS NOT NULL AND supplier_id = var_supplier_id;

    IF var_total_debt_produk IS NULL THEN
        var_total_debt_produk := 0;  
    END IF; 
        
    IF var_total_settlement_produk IS NULL THEN
        var_total_settlement_produk := 0;  
    END IF;
                
    UPDATE m_supplier SET total_debt = var_total_debt_produk, total_debt_payment = var_total_settlement_produk 
    WHERE supplier_id = var_supplier_id;       
    
    IF TG_OP = 'UPDATE' THEN
    	var_supplier_id_old := OLD.supplier_id;
        
    	IF var_supplier_id <> var_supplier_id_old THEN
            -- hitung total hutang dan pelunasan pembelian produk
            SELECT SUM(total_invoice - discount + tax) AS total_debt, SUM(total_payment) AS total_payment
            INTO var_total_debt_produk, var_total_settlement_produk
            FROM t_purchase_product             
            WHERE date_tempo IS NOT NULL AND supplier_id = var_supplier_id_old;

            IF var_total_debt_produk IS NULL THEN
                var_total_debt_produk := 0;  
            END IF; 
                
            IF var_total_settlement_produk IS NULL THEN
                var_total_settlement_produk := 0;  
            END IF;
                
            UPDATE m_supplier SET total_debt = var_total_debt_produk, total_debt_payment = var_total_settlement_produk 
            WHERE supplier_id = var_supplier_id_old;                                    
    		                        
            UPDATE t_product_payable_payment SET supplier_id = var_supplier_id
            WHERE pembayaran_hutang_product_id IN (SELECT pembayaran_hutang_product_id FROM t_debt_payment_item WHERE beli_product_id = NEW.beli_product_id);                
            
        END IF;
    END IF;            
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_supplier_total_debt() OWNER TO postgres;

--
-- Name: f_update_total_sales(); Type: FUNCTION; Schema: public; Owner: postgres
--
CREATE FUNCTION f_update_total_sales() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
	var_sale_id 			t_guid;
    var_return_sale_id		t_guid;
    var_diskon				t_price;
    var_ppn					t_price;
	var_shipping_cost		t_price;
	var_total_nota 			t_price;        
    var_return_quantity_lama 	t_quantity;
    var_return_quantity_baru 	t_quantity;
    var_due_date		DATE;
    is_retur				t_bool;
    
BEGIN
	is_retur := FALSE;
    
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_sale_id := NEW.sale_id;    
    ELSE
    	var_sale_id := OLD.sale_id;        
    END IF;	    	          

    var_total_nota := (SELECT SUM((quantity - return_quantity) * (selling_price - (discount / 100 * selling_price))) 
    				   FROM t_sales_order_item
					   WHERE sale_id = var_sale_id);	    
	IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;     
    
    var_total_nota := ROUND(var_total_nota, 0);      
    
	SELECT tax, shipping_cost, discount INTO var_ppn, var_shipping_cost, var_diskon
    FROM t_product_sales WHERE sale_id = var_sale_id;            
    
    IF var_ppn IS NULL THEN
    	var_ppn := 0;  
	END IF;
    
	IF var_shipping_cost IS NULL THEN
    	var_shipping_cost := 0;  
	END IF;
	
    IF var_diskon IS NULL THEN
    	var_diskon := 0;  
	END IF;
    
    IF TG_OP = 'UPDATE' THEN -- pengecekan retur
    	-- quantity retur
        var_return_quantity_baru = NEW.return_quantity;
        var_return_quantity_lama = OLD.return_quantity;        
        
    	IF var_return_quantity_lama <> var_return_quantity_baru THEN
			is_retur := TRUE;
			
            var_return_sale_id := (SELECT return_sale_id FROM t_sales_return WHERE sale_id = var_sale_id LIMIT 1);
            var_due_date := (SELECT due_date FROM t_product_sales WHERE sale_id = var_sale_id);                                                
            
            IF var_due_date IS NULL THEN -- nota tunai            	
                UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), return_sale_id = var_return_sale_id 
                WHERE sale_id = var_sale_id;                
                
                UPDATE t_credit_payment_item SET amount = ROUND(var_total_nota, 0) - var_diskon + var_ppn + var_shipping_cost
                WHERE sale_id = var_sale_id;
            ELSE
            	UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), return_sale_id = var_return_sale_id 
                WHERE sale_id = var_sale_id;
            END IF;        
        END IF;        
    END IF;              

	IF (is_retur = FALSE) THEN -- bukan retur    	
    	UPDATE t_product_sales SET total_invoice = ROUND(var_total_nota, 0), return_sale_id = NULL 
        WHERE sale_id = var_sale_id;
    END IF;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_total_sales() OWNER TO postgres;
--
-- Name: f_update_employee_total_loan(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_employee_total_loan() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 
    var_employee_id		t_guid;
    
	var_total_loan 	t_price;
  	var_total_settlement	t_price;
    
BEGIN
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_employee_id := NEW.employee_id;    
    ELSE
    	var_employee_id := OLD.employee_id;
    END IF;	            
	
    SELECT SUM(amount), SUM(total_payment)
    INTO var_total_loan, var_total_settlement
	FROM t_loan WHERE employee_id = var_employee_id;
        
    IF var_total_loan IS NULL THEN
    	var_total_loan := 0;  
	END IF;
    
    IF var_total_settlement IS NULL THEN
    	var_total_settlement := 0;  
	END IF;
        
    UPDATE m_employee SET total_loan = var_total_loan, total_loan_payment = var_total_settlement 
    WHERE employee_id = var_employee_id;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_employee_total_loan() OWNER TO postgres;

--
-- Name: f_update_total_expenses(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_total_expenses() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE v_expense_id t_guid;
DECLARE v_total t_price;
  
BEGIN
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	v_expense_id := NEW.expense_id;    
    ELSE
    	v_expense_id := OLD.expense_id;
    END IF;
        
    v_total := (SELECT SUM(quantity * price) FROM t_expense_item
			    WHERE expense_id = v_expense_id);
	
    IF v_total IS NULL THEN
    	v_total := 0;  
	END IF;
    
    UPDATE t_expenceSET total = v_total WHERE expense_id = v_expense_id;                        
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_total_expenses() OWNER TO postgres;

--
-- Name: f_update_customer_total_credit(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_customer_total_credit() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 	
    var_total_credit	t_price;
    var_total_settlement	t_price;
  	var_customer_id		t_guid;
    var_customer_id_old	t_guid;
    
BEGIN
	IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_customer_id := NEW.customer_id;        
    ELSE
    	var_customer_id := OLD.customer_id;
        var_customer_id_old = OLD.customer_id;
    END IF;
	    	            
	SELECT SUM(total_invoice - discount + shipping_cost + tax), SUM(total_payment)
    INTO var_total_credit, var_total_settlement
	FROM t_product_sales    
    WHERE date_tempo IS NOT NULL AND customer_id = var_customer_id;
    
    IF var_total_credit IS NULL THEN
        var_total_credit := 0;  
    END IF;
    
    IF var_total_settlement IS NULL THEN
        var_total_settlement := 0;  
    END IF;
        
    UPDATE m_customer SET total_credit = var_total_credit, total_receivable_payment = var_total_settlement
	WHERE customer_id = var_customer_id;        
    
    IF TG_OP = 'UPDATE' THEN
		var_customer_id_old = OLD.customer_id;
		
        IF var_customer_id <> var_customer_id_old THEN
        	SELECT SUM(total_invoice - discount + shipping_cost + tax), SUM(total_payment)
            INTO var_total_credit, var_total_settlement
            FROM t_product_sales            
            WHERE date_tempo IS NOT NULL AND customer_id = var_customer_id_old;
            
            IF var_total_credit IS NULL THEN
                var_total_credit := 0;  
            END IF;
            
            IF var_total_settlement IS NULL THEN
                var_total_settlement := 0;  
            END IF;                           
			
			UPDATE m_customer SET total_credit = var_total_credit, total_receivable_payment = var_total_settlement
            WHERE customer_id = var_customer_id_old;
            
            UPDATE t_product_receivable_paymentSET customer_id = var_customer_id 
            WHERE pay_sale_id IN (SELECT pay_sale_id FROM t_credit_payment_itemWHERE sale_id = NEW.sale_id);
        END IF;
    END IF;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_customer_total_credit() OWNER TO postgres;

--
-- Name: f_update_purchase_return_total_aiud(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_purchase_return_total_aiud() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
	var_purchase_return_id	t_guid;
	var_total_nota 		t_price;
    
BEGIN    
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_purchase_return_id := NEW.purchase_return_id;    
    ELSE
    	var_purchase_return_id := OLD.purchase_return_id;
    END IF;
        
    var_total_nota := (SELECT SUM(return_quantity * price) 
    				   FROM t_purchase_return_item
					   WHERE purchase_return_id = var_purchase_return_id);
                           
    IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;                         
    
    UPDATE t_purchase_return SET total_invoice = var_total_nota 
    WHERE purchase_return_id = var_purchase_return_id;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_purchase_return_total_aiud() OWNER TO postgres;


--
-- Name: f_update_sales_return_total_aiud(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION f_update_sales_return_total_aiud() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
	var_return_sale_id	t_guid;
	var_total_nota 		t_price;
    
BEGIN    
    IF TG_OP = 'INSERT' OR TG_OP = 'UPDATE' THEN
    	var_return_sale_id := NEW.return_sale_id;    
    ELSE
    	var_return_sale_id := OLD.return_sale_id;
    END IF;
        
    var_total_nota := (SELECT SUM(return_quantity * selling_price) 
    				   FROM t_sales_return_item
					   WHERE return_sale_id = var_return_sale_id);
                           
    IF var_total_nota IS NULL THEN
    	var_total_nota := 0;  
	END IF;                         
    
    UPDATE t_sales_return SET total_invoice = var_total_nota 
    WHERE return_sale_id = var_return_sale_id;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.f_update_sales_return_total_aiud() OWNER TO postgres;
--
-- Name: fn_log_last_update(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION fn_log_last_update() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE 	
    var_product_id		t_guid;
    var_iselling_price		t_price;
    var_iselling_price_old	t_price;
    
BEGIN
	IF TG_OP = 'UPDATE' THEN
    	var_product_id := NEW.product_id;
        var_iselling_price := NEW.iselling_price;        
        var_iselling_price_old := OLD.iselling_price;    	
    END IF;
    
    IF var_iselling_price IS NULL THEN
    	var_iselling_price := 0;
    END IF;
    	   
    IF var_iselling_price_old IS NULL THEN
    	var_iselling_price_old := 0;
    END IF;
    
    IF var_iselling_price_old <> var_iselling_price THEN
    	UPDATE m_product SET last_update = now() WHERE product_id = var_product_id;
    END IF;
    
    RETURN NULL;
END;
$$;


ALTER FUNCTION public.fn_log_last_update() OWNER TO postgres;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: m_adjustment_reason; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_adjustment_reason(
    stock_adjustment_reason_id t_guid NOT NULL,
    reason t_description
);


ALTER TABLE m_adjustment_reason OWNER TO postgres;

--
-- Name: m_customer; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_customer (
    customer_id t_guid NOT NULL,
    name_customer t_name,
    address t_address_panjang,
    contact t_name,
    phone t_phone,
    credit_limit t_price,
    total_credit t_price,
    total_receivable_payment t_price,
    subdistrict t_address,
    city t_address,
    postal_code t_postal_code,
    discount t_quantity,
    country t_address,
    regency t_address,
    province_id character(2),
    regency_id character(4),
    subdistrict_id character(7),
	village t_address
);


ALTER TABLE m_customer OWNER TO postgres;

--
-- Name: m_database_version; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_database_version (
    version_number integer NOT NULL
);


ALTER TABLE m_database_version OWNER TO postgres;

--
-- Name: m_dropshipper; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_dropshipper (
    dropshipper_id t_guid NOT NULL,
    name_dropshipper t_name,
    address t_address,
    contact t_name,
    phone t_phone
);


ALTER TABLE m_dropshipper OWNER TO postgres;

--
-- Name: m_mini_pos_invoice_footer; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_mini_pos_invoice_footer (
    footer_invoice_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_mini_pos_invoice_footer OWNER TO postgres;

--
-- Name: m_category; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--
F
CREATE TABLE m_category (
    category_id t_guid NOT NULL,
    name_category t_name,
    discount t_quantity,
    profit_percentage t_quantity
);


ALTER TABLE m_category OWNER TO postgres;

--
-- Name: m_wholesale_price; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_wholesale_price (
    wholesale_price_id t_guid NOT NULL,
    product_id t_guid,
    retail_price integer,
    wholesale_price t_price,
    minimum_quantity t_quantity,
    discount t_quantity
);


ALTER TABLE m_wholesale_price OWNER TO postgres;

--
-- Name: m_invoice_header; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_header (
    header_invoice_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_invoice_header OWNER TO postgres;

--
-- Name: m_invoice_header_mini_pos; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_header_mini_pos (
    header_invoice_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_invoice_header_mini_pos OWNER TO postgres;

--
-- Name: m_item_menu; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_item_menu (
    item_menu_id t_guid NOT NULL,
    menu_id t_guid,
    grant_id integer,
    description t_description
);


ALTER TABLE m_item_menu OWNER TO postgres;

--
-- Name: COLUMN m_item_menu.grant_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_item_menu.grant_id IS 'Mereference ke tabel m_role_privilege';


--
-- Name: m_job_titles; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_job_titles (
    job_titles_id t_guid NOT NULL,
    name_job_titles t_name,
    description t_description
);


ALTER TABLE m_job_titles OWNER TO postgres;

--
-- Name: m_expense_type; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_expense_type (
    expense_type_id t_guid NOT NULL,
    name_expense_type t_description
);


ALTER TABLE m_expense_type OWNER TO postgres;

--
-- Name: m_regency; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_regency (
    regency_id integer NOT NULL,
    province_id integer,
    type character varying(15),
    name_regency t_description,
    postal_code t_postal_code
);


ALTER TABLE m_regency OWNER TO postgres;

--
-- Name: m_regency2; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_regency2 (
    regency_id character(4) NOT NULL,
    province_id character(2),
    name_regency t_address_panjang
);


ALTER TABLE m_regency2 OWNER TO postgres;

--
-- Name: m_card; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_card (
    card_id t_guid NOT NULL,
    card_name t_name,
    is_debit t_bool
);


ALTER TABLE m_card OWNER TO postgres;

--
-- Name: m_employee; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_employee (
    employee_id t_guid NOT NULL,
    job_titles_id t_guid,
    employee_name t_name,
    address t_address,
    phone t_phone,
    basic_salary t_price,
    is_active t_bool,
    description t_description,
    payment_type integer DEFAULT 1,
    overtime_salary t_price DEFAULT 0,
    total_loan t_price,
    total_loan_payment t_price
);


ALTER TABLE m_employee OWNER TO postgres;

--
-- Name: m_subdistrict; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_subdistrict (
    subdistrict_id character(7) NOT NULL,
    regency_id character(4),
    name_subdistrict t_address_panjang
);


ALTER TABLE m_subdistrict OWNER TO postgres;

--
-- Name: m_invoice_label; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_label (
    invoice_label_id t_guid NOT NULL,
    description t_description,
    order_number integer,
    is_active t_bool
);


ALTER TABLE m_invoice_label OWNER TO postgres;

--
-- Name: m_menu; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_menu (
    menu_id t_guid NOT NULL,
    name_menu t_name,
    menu_title t_description,
    parent_id t_guid,
    order_number integer,
    is_active t_bool,
    name_form t_description,
    is_enabled t_bool
);


ALTER TABLE m_menu OWNER TO postgres;

--
-- Name: COLUMN m_menu.parent_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_menu.parent_id IS 'Diisi dengan menu_id';


--
-- Name: m_user; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_user (
    user_id t_guid NOT NULL,
    role_id t_guid,
    name_user t_name,
    user_password t_password,
    is_active t_bool,
    status_user integer DEFAULT 2,
    email t_description
);


ALTER TABLE m_user OWNER TO postgres;

--
-- Name: COLUMN m_user.status_user; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_user.status_user IS '1 = Kasir
2 = Server
3 = Kasir dan Server';


--
-- Name: m_invoice_prefix; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_invoice_prefix (
    prefix_invoice_id integer DEFAULT 1 NOT NULL,
    invoice_prefix character varying(3),
    description t_description
);


ALTER TABLE m_invoice_prefix OWNER TO postgres;

--
-- Name: m_product; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_product (
    product_id t_guid NOT NULL,
    product_name t_name_panjang,
    unit t_unit,
    stock t_quantity,
    purchase_price t_price,
    selling_price t_price,
    product_code t_product_code,
    category_id t_guid,
    minimal_stock t_quantity DEFAULT 0,
    warehouse_stock t_quantity,
    minimal_stock_warehouse t_quantity,
    discount t_quantity,
    profit_percentage t_quantity,
    is_active t_bool,
    last_update timestamp(0) without time zone DEFAULT now()
);


ALTER TABLE m_product OWNER TO postgres;

--
-- Name: m_product_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE m_product_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE m_product_product_id_seq OWNER TO postgres;

--
-- Name: m_profile; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_profile (
    profile_id t_guid NOT NULL,
    name_profile t_description,
    address t_address,
    city t_address,
    phone t_phone,
    email t_description,
    website t_description,
    register_id t_guid,
    is_register boolean DEFAULT false,
    hash t_description
);


ALTER TABLE m_profile OWNER TO postgres;

--
-- Name: m_province; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_province (
    province_id integer NOT NULL,
    name_province t_description
);


ALTER TABLE m_province OWNER TO postgres;

--
-- Name: m_province2; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_province2 (
    province_id character(2) NOT NULL,
    name_province t_address_panjang
);


ALTER TABLE m_province2 OWNER TO postgres;

--
-- Name: m_role; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_role (
    role_id t_guid NOT NULL,
    name_role t_name,
    is_active t_bool
);


ALTER TABLE m_role OWNER TO postgres;

--
-- Name: m_role_privilege; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_role_privilege (
    role_id t_guid NOT NULL,
    menu_id t_guid NOT NULL,
    grant_id integer NOT NULL,
    is_grant t_bool
);


ALTER TABLE m_role_privilege OWNER TO postgres;

--
-- Name: COLUMN m_role_privilege.grant_id; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN m_role_privilege.grant_id IS 'Tambah, Perbaiki, Hapus, Dll';


--
-- Name: m_application_setting; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_application_setting (
    application_setting_id t_guid NOT NULL,
    is_update_iselling_price_master_produk t_bool,
    is_negative_stock_allowed_for_products t_bool,
    is_focus_on_inputting_quantity_column t_bool,
    is_tampilkan_description_tambahan_item_jual t_bool,
    description_tambahan_item_jual t_description
);


ALTER TABLE m_application_setting OWNER TO postgres;

--
-- Name: m_shift; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_shift (
    shift_id t_guid NOT NULL,
    name_shift t_description,
    start_time timestamp without time zone,
    end_time timestamp without time zone,
    is_active t_bool
);


ALTER TABLE m_shift OWNER TO postgres;

--
-- Name: m_supplier; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE m_supplier (
    supplier_id t_guid NOT NULL,
    name_supplier t_name,
    address t_address,
    contact t_name,
    phone t_phone,
    total_debt t_price,
    total_debt_payment t_price
);


ALTER TABLE m_supplier OWNER TO postgres;

--
-- Name: t_purchase_product; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_product (
    purchase_id t_guid NOT NULL,
    user_id t_guid,
    supplier_id t_guid,
    purchase_return_id t_guid,
    nota t_invoice,
    date date,
    date_tempo date,
    tax t_price,
    discount t_price,
    total_invoice t_price,
    total_payment t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_purchase_product OWNER TO postgres;

--
-- Name: t_purchase_product_beli_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_purchase_product_beli_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_purchase_product_beli_product_id_seq OWNER TO postgres;

--
-- Name: t_employee_salary; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_employee_salary (
    gaji_employee_id t_guid NOT NULL,
    employee_id t_guid,
    user_id t_guid,
    month integer,
    year integer,
    attendance integer,
    absence integer,
    basic_salary t_price,
    overtime t_price,
    bonus t_price,
    deductions t_price,
    system_date timestamp without time zone DEFAULT now(),
    time integer DEFAULT 0,
    other t_price DEFAULT 0,
    description t_description,
    days_worked integer DEFAULT 0,
    allowance t_price DEFAULT 0,
    loan t_price DEFAULT 0,
    date date,
    invoice t_invoice
);


ALTER TABLE t_employee_salary OWNER TO postgres;

--
-- Name: t_employee_salary_gaji_employee_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_employee_salary_gaji_employee_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_employee_salary_gaji_employee_id_seq OWNER TO postgres;

--
-- Name: t_purchase_order_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_order_item (
     purchase_item_id t_guid NOT NULL,
    purchase_id t_guid,
    user_id t_guid,
    product_id t_guid,
    price t_price,
    quantity t_quantity,
    discount t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity
);


ALTER TABLE t_purchase_order_item OWNER TO postgres;

--
-- Name: t_sales_order_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_sales_order_item (
    item_sale_id t_guid NOT NULL,
    sale_id t_guid,
    user_id t_guid,
    product_id t_guid,
    purchase_price t_price,
    iselling_price t_price,
    quantity t_quantity,
    discount t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity,
    description t_description
);


ALTER TABLE t_sales_order_item OWNER TO postgres;

--
-- Name: t_debt_payment_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_debt_payment_item (
    pay_purchase_item_id t_guid NOT NULL,
    pay_purchase_id t_guid,
    purchase_id t_guid,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_debt_payment_item OWNER TO postgres;

--
-- Name: t_credit_payment_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_credit_payment_item(
    pay_sale_item_id t_guid NOT NULL,
    pay_sale_id t_guid,
    sale_id t_guid,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_credit_payment_item OWNER TO postgres;

--
-- Name: t_expense_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_expense_item(
    expense_item_id t_guid NOT NULL,
    expense_id t_guid,
    user_id t_guid,
    quantity t_quantity,
    price t_price,
    system_date timestamp without time zone DEFAULT now(),
    expense_type_id t_guid
);


ALTER TABLE t_expense_item OWNER TO postgres;

--
-- Name: t_purchase_return_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_return_item(
    return_purchase_item_id t_guid NOT NULL,
    purchase_return_id t_guid,
    user_id t_guid,
    product_id t_guid,
    price t_price,
    quantity t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity,
    purchase_item_id t_guid
);


ALTER TABLE t_purchase_return_item OWNER TO postgres;

--
-- Name: t_sales_return_item; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_sales_return_item(
    return_sale_item_id t_guid NOT NULL,
    return_sale_id t_guid,
    user_id t_guid,
    product_id t_guid,
    selling_price t_price,
    quantity t_quantity,
    system_date timestamp without time zone DEFAULT now(),
    return_quantity t_quantity,
    sale_item_id t_guid
);


ALTER TABLE t_sales_return_item OWNER TO postgres;

--
-- Name: t_product_sales; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_product_sales(
    sale_id t_guid NOT NULL,
    user_id t_guid,
    customer_id t_guid,
    invoice t_invoice,
    date date,
    date_tempo date,
    tax t_price,
    discount t_price,
    total_invoice t_price,
    total_payment t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    return_sale_id t_guid,
    shift_id t_guid,
    is_sdac t_bool,
    shipping_subdistrict t_address_panjang,
    shipping_city t_address_panjang,
    shipping_postal_code t_postal_code,
    shipping_kepada t_name,
    shipping_address t_address_panjang,
    shipping_phone t_phone,
    shipping_cost t_price,
	shipping_to text,
	shipping_village text,
    from_label1 t_description,
    from_label2 t_description,
    from_label3 t_description,
    from_label4 t_description,
    to_label1 t_address_panjang,
    to_label2 t_address_panjang,
    to_label3 t_address_panjang,
    to_label4 t_address_panjang,
    courier t_description,
    is_dropship boolean,
    shipping_country t_address_panjang,
    shipping_regency t_address_panjang,
    machine_id t_guid,
    payment_cash t_price,
    payment_card t_price,
    card_id t_guid,
    card_number t_invoice,
    dropshipper_id t_guid
);


ALTER TABLE t_product_sales OWNER TO postgres;

--
-- Name: COLUMN t_product_sales.is_sdac; Type: COMMENT; Schema: public; Owner: postgres
--

COMMENT ON COLUMN t_product_sales.is_sdac IS 'Sama dengan address customer';


--
-- Name: t_product_sales_jual_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_product_sales_jual_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_sales_jual_product_id_seq OWNER TO postgres;

--
-- Name: t_loan; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_loan (
    loan_id t_guid NOT NULL,
    employee_id t_guid,
    user_id t_guid,
    invoice t_invoice,
    date date,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_payment t_price DEFAULT 0
);


ALTER TABLE t_loan OWNER TO postgres;

--
-- Name: t_loan_loan_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_loan_loan_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_loan_loan_id_seq OWNER TO postgres;

--
-- Name: t_logs; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_logs (
    log_id bigint DEFAULT nextval(('public.t_logs_log_id_seq'::text)::regclass) NOT NULL,
    level character varying(10),
    class_name character varying(200),
    method_name character varying(100),
    message character varying(100),
    new_value character varying(10000),
    old_value character varying(10000),
    exception character varying(10000),
    created_by character varying(50),
    log_date timestamp(0) without time zone DEFAULT now()
);


ALTER TABLE t_logs OWNER TO postgres;

--
-- Name: t_logs_log_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_logs_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_logs_log_id_seq OWNER TO postgres;

--
-- Name: t_machine; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_machine(
    machine_id t_guid NOT NULL,
    user_id t_guid,
    date date DEFAULT ('now'::text)::date,
    starting_balance t_price,
    cash_in t_price,
    system_date timestamp without time zone DEFAULT now(),
    shift_id t_guid,
    cash_out t_price
);


ALTER TABLE t_machine OWNER TO postgres;

--
-- Name: t_product_payable_payment; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_product_payable_payment (
    pay_purchase_id t_guid NOT NULL,
    supplier_id t_guid,
    user_id t_guid,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    invoice t_invoice,
    is_cash t_bool
);


ALTER TABLE t_product_payable_payment OWNER TO postgres;

--
-- Name: t_product_payable_payment_pembayaran_hutang_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_product_payable_payment_pembayaran_hutang_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_payable_payment_pembayaran_hutang_product_id_seq OWNER TO postgres;

--
-- Name: t_pembayaran_loan; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_pembayaran_loan (
    pembayaran_loan_id t_guid NOT NULL,
    loan_id t_guid,
    gaji_employee_id t_guid,
    date date,
    amount t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    nota t_invoice,
    user_id t_guid
);


ALTER TABLE t_pembayaran_loan OWNER TO postgres;

--
-- Name: t_pembayaran_loan_pembayaran_loan_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_pembayaran_loan_pembayaran_loan_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_pembayaran_loan_pembayaran_loan_id_seq OWNER TO postgres;

--
-- Name: t_product_receivable_payment; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_product_receivable_payment(
    pay_sale_id t_guid NOT NULL,
    customer_id t_guid,
    user_id t_guid,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    invoice t_invoice,
    is_cash t_bool
);


ALTER TABLE t_product_receivable_payment OWNER TO postgres;

--
-- Name: t_product_receivable_payment_pembayaran_piutang_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_product_receivable_payment_pembayaran_piutang_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_product_receivable_payment_pembayaran_piutang_product_id_seq OWNER TO postgres;

--
-- Name: t_expence; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_expence(
    expense_id t_guid NOT NULL,
    user_id t_guid,
    invoice t_invoice,
    date date,
    total t_price,
    description t_description,
    system_date timestamp without time zone DEFAULT now()
);


ALTER TABLE t_expence OWNER TO postgres;

--
-- Name: t_expence_pengeluaran_biaya_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_expence_pengeluaran_biaya_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_expence_pengeluaran_biaya_id_seq OWNER TO postgres;

--
-- Name: t_stock_adjustment; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_stock_adjustment (
    stock_adjustment_id t_guid NOT NULL,
    product_id t_guid,
    adjustment_reason_id t_guid,
    date date,
    stock_addition t_quantity,
    stock_reduction t_quantity,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    penambahan_warehouse_stock t_quantity,
    pengurangan_warehouse_stock t_quantity
);
ALTER TABLE ONLY t_stock_adjustment ALTER COLUMN adjustment_reason_id SET STATISTICS 0;


ALTER TABLE t_stock_adjustment OWNER TO postgres;

--
-- Name: t_purchase_return; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_purchase_return (
    purchase_return_id t_guid NOT NULL,
    beli_product_id t_guid,
    user_id t_guid,
    supplier_id t_guid,
    invoice t_invoice,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_invoice t_price
);


ALTER TABLE t_purchase_return OWNER TO postgres;

--
-- Name: t_purchase_return_retur_beli_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_purchase_return_retur_beli_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_purchase_return_retur_beli_product_id_seq OWNER TO postgres;

--
-- Name: t_sales_return; Type: TABLE; Schema: public; Owner: postgres; Tablespace: 
--

CREATE TABLE t_sales_return (
    return_sale_id t_guid NOT NULL,
    sale_id t_guid,
    user_id t_guid,
    customer_id t_guid,
    invoice t_invoice,
    date date,
    description t_description,
    system_date timestamp without time zone DEFAULT now(),
    total_invoice t_price
);


ALTER TABLE t_sales_return OWNER TO postgres;

--
-- Name: t_sales_return_retur_jual_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE t_sales_return_retur_jual_product_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE t_sales_return_retur_jual_product_id_seq OWNER TO postgres;

--
-- Name: m_adjustment_reason_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_adjustment_reason
    ADD CONSTRAINT m_adjustment_reason_pkey PRIMARY KEY (stock_adjustment_reason_id);


--
-- Name: m_customer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_customer
    ADD CONSTRAINT m_customer_pkey PRIMARY KEY (customer_id);


--
-- Name: m_database_version_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_database_version
    ADD CONSTRAINT m_database_version_pkey PRIMARY KEY (version_number);


--
-- Name: m_dropshipper_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_dropshipper
    ADD CONSTRAINT m_dropshipper_pkey PRIMARY KEY (dropshipper_id);


--
-- Name: m_mini_pos_invoice_footer_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_mini_pos_invoice_footer
    ADD CONSTRAINT m_mini_pos_invoice_footer_pkey PRIMARY KEY (footer_invoice_id);


--
-- Name: m_category_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_category
    ADD CONSTRAINT m_category_pkey PRIMARY KEY (category_id);


--
-- Name: m_wholesale_price_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_wholesale_price
    ADD CONSTRAINT m_wholesale_price_pkey PRIMARY KEY (wholesale_price_id);


--
-- Name: m_invoice_header_mini_pos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_header_mini_pos
    ADD CONSTRAINT m_invoice_header_mini_pos_pkey PRIMARY KEY (header_invoice_id);


--
-- Name: m_invoice_header_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_header
    ADD CONSTRAINT m_invoice_header_pkey PRIMARY KEY (header_invoice_id);


--
-- Name: m_item_menu_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_item_menu
    ADD CONSTRAINT m_item_menu_pkey PRIMARY KEY (item_menu_id);


--
-- Name: m_job_titles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_job_titles
    ADD CONSTRAINT m_job_titles_pkey PRIMARY KEY (job_titles_id);


--
-- Name: m_expense_type_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_expense_type
    ADD CONSTRAINT m_expense_type_pkey PRIMARY KEY (expense_type_id);


--
-- Name: m_regency2_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_regency2
    ADD CONSTRAINT m_regency2_pkey PRIMARY KEY (regency_id);


--
-- Name: m_regency_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_regency
    ADD CONSTRAINT m_regency_pkey PRIMARY KEY (regency_id);


--
-- Name: m_card_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_card
    ADD CONSTRAINT m_card_pkey PRIMARY KEY (card_id);


--
-- Name: m_employee_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_employee
    ADD CONSTRAINT m_employee_pkey PRIMARY KEY (employee_id);


--
-- Name: m_subdistrict_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_subdistrict
    ADD CONSTRAINT m_subdistrict_pkey PRIMARY KEY (subdistrict_id);


--
-- Name: m_invoice_label_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_label
    ADD CONSTRAINT m_invoice_label_pkey PRIMARY KEY (invoice_label_id);


--
-- Name: m_menu_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_menu
    ADD CONSTRAINT m_menu_pkey PRIMARY KEY (menu_id);


--
-- Name: m_user_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_user
    ADD CONSTRAINT m_user_pkey PRIMARY KEY (user_id);


--
-- Name: m_invoice_prefix_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_invoice_prefix
    ADD CONSTRAINT m_invoice_prefix_pkey PRIMARY KEY (prefix_invoice_id);


--
-- Name: m_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_product
    ADD CONSTRAINT m_product_pkey PRIMARY KEY (product_id);


--
-- Name: m_profile_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_profile
    ADD CONSTRAINT m_profile_pkey PRIMARY KEY (profile_id);


--
-- Name: m_provinsi2_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_province2
    ADD CONSTRAINT m_provinsi2_pkey PRIMARY KEY (province_id);


--
-- Name: m_provinsi_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_province
    ADD CONSTRAINT m_provinsi_pkey PRIMARY KEY (province_id);


--
-- Name: m_role_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_role
    ADD CONSTRAINT m_role_pkey PRIMARY KEY (role_id);


--
-- Name: m_role_privilege_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_role_privilege
    ADD CONSTRAINT m_role_privilege_pkey PRIMARY KEY (role_id, menu_id, grant_id);


--
-- Name: m_application_setting_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_application_setting
    ADD CONSTRAINT m_application_setting_pkey PRIMARY KEY (application_setting_id);


--
-- Name: m_shift_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_shift
    ADD CONSTRAINT m_shift_pkey PRIMARY KEY (shift_id);


--
-- Name: m_supplier_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY m_supplier
    ADD CONSTRAINT m_supplier_pkey PRIMARY KEY (supplier_id);


--
-- Name: t_purchase_product_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_pkey PRIMARY KEY (beli_product_id);


--
-- Name: t_employee_salary_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_employee_salary
    ADD CONSTRAINT t_employee_salary_pkey PRIMARY KEY (gaji_employee_id);


--
-- Name: t_purchase_order_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_pkey PRIMARY KEY (item_beli_product_id);


--
-- Name: t_item_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_pkey PRIMARY KEY (item_sale_id);


--
-- Name: t_debt_payment_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_pkey PRIMARY KEY (item_pembayaran_hutang_product_id);


--
-- Name: t_item_pembayaran_piutang_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_credit_payment_item
    ADD CONSTRAINT t_item_pembayaran_piutang_pkey PRIMARY KEY (pay_sale_item_id);


--
-- Name: t_item_pengeluaran_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_pkey PRIMARY KEY (expense_item_id);


--
-- Name: t_purchase_return_item_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_pkey PRIMARY KEY (item_retur_beli_product_id);


--
-- Name: t_item_retur_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_pkey PRIMARY KEY (item_retur_sale_id);


--
-- Name: t_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_pkey PRIMARY KEY (sale_id);


--
-- Name: t_loan_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_loan
    ADD CONSTRAINT t_loan_pkey PRIMARY KEY (loan_id);


--
-- Name: t_logs_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_logs
    ADD CONSTRAINT t_logs_pkey PRIMARY KEY (log_id);


--
-- Name: t_machine_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_machine
    ADD CONSTRAINT t_machine_pkey PRIMARY KEY (machine_id);


--
-- Name: t_pembayaran_bon_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_bon_pkey PRIMARY KEY (pembayaran_loan_id);


--
-- Name: t_product_payable_payment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_product_payable_payment
    ADD CONSTRAINT t_product_payable_payment_pkey PRIMARY KEY (pembayaran_hutang_product_id);


--
-- Name: t_pembayaran_piutang_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_product_receivable_payment
    ADD CONSTRAINT t_pembayaran_piutang_pkey PRIMARY KEY (pay_sale_id);


--
-- Name: t_pengeluaran_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_expence
    ADD CONSTRAINT t_pengeluaran_pkey PRIMARY KEY (expense_id);


--
-- Name: t_stock_adjustment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_stock_adjustment
    ADD CONSTRAINT t_stock_adjustment_pkey PRIMARY KEY (stock_adjustment_id);


--
-- Name: t_purchase_return_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_pkey PRIMARY KEY (retur_beli_product_id);


--
-- Name: t_retur_jual_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres; Tablespace: 
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_pkey PRIMARY KEY (retur_sale_id);


--
-- Name: m_customer_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_customer_idx ON m_customer USING btree (name_customer);


--
-- Name: m_regency2_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_regency2_idx ON m_regency2 USING btree (province_id);


--
-- Name: m_regency2_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_regency2_idx1 ON m_regency2 USING btree (name_regency);


--
-- Name: m_regency_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_regency_idx ON m_regency USING btree (name_regency);


--
-- Name: m_subdistrict_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_subdistrict_idx ON m_subdistrict USING btree (regency_id);


--
-- Name: m_subdistrict_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_subdistrict_idx1 ON m_subdistrict USING btree (name_subdistrict);


--
-- Name: m_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx ON m_product USING btree (product_name);


--
-- Name: m_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx1 ON m_product USING btree (product_code);


--
-- Name: m_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx2 ON m_product USING btree (category_id);


--
-- Name: m_product_idx3; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_product_idx3 ON m_product USING btree (is_active);


--
-- Name: m_provinsi2_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_provinsi2_idx ON m_province2 USING btree (name_province);


--
-- Name: m_province_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_province_idx ON m_province USING btree (name_province);


--
-- Name: m_supplier_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX m_supplier_idx ON m_supplier USING btree (name_supplier);


--
-- Name: t_beli_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx ON t_purchase_product USING btree (date);


--
-- Name: t_beli_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx1 ON t_purchase_product USING btree (nota);


--
-- Name: t_beli_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx2 ON t_purchase_product USING btree (date_tempo);


--
-- Name: t_beli_product_idx3; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx3 ON t_purchase_product USING btree (supplier_id);


--
-- Name: t_beli_product_idx4; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_beli_product_idx4 ON t_purchase_product USING btree (user_id);


--
-- Name: t_gaji_employee_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_gaji_employee_idx ON t_employee_salary USING btree (month, year);


--
-- Name: t_gaji_employee_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_gaji_employee_idx1 ON t_employee_salary USING btree (date);


--
-- Name: t_gaji_employee_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_gaji_employee_idx2 ON t_employee_salary USING btree (nota);


--
-- Name: t_jual_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx ON t_product_sales USING btree (nota);


--
-- Name: t_jual_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx1 ON t_product_sales USING btree (date);


--
-- Name: t_jual_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx2 ON t_product_sales USING btree (date_tempo);


--
-- Name: t_jual_product_idx3; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx3 ON t_product_sales USING btree (customer_id);


--
-- Name: t_jual_product_idx4; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_jual_product_idx4 ON t_product_sales USING btree (user_id);


--
-- Name: t_loan_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_loan_idx ON t_loan USING btree (date);


--
-- Name: t_loan_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_loan_idx1 ON t_loan USING btree (nota);


--
-- Name: t_machine_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_machine_idx ON t_machine USING btree (date);


--
-- Name: t_machine_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_machine_idx1 ON t_machine USING btree (user_id);


--
-- Name: t_pembayaran_hutang_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_hutang_product_idx ON t_product_payable_payment USING btree (date);


--
-- Name: t_pembayaran_hutang_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_hutang_product_idx1 ON t_product_payable_payment USING btree (nota);


--
-- Name: t_pembayaran_hutang_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_hutang_product_idx2 ON t_product_payable_payment USING btree (supplier_id);


--
-- Name: t_pembayaran_loan_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_loan_idx ON t_pembayaran_loan USING btree (date);


--
-- Name: t_pembayaran_loan_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_loan_idx1 ON t_pembayaran_loan USING btree (nota);


--
-- Name: t_pembayaran_piutang_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx ON t_product_receivable_payment USING btree (date);


--
-- Name: t_pembayaran_piutang_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx1 ON t_product_receivable_payment USING btree (nota);


--
-- Name: t_pembayaran_piutang_product_idx2; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_pembayaran_piutang_product_idx2 ON t_product_receivable_payment USING btree (customer_id);


--
-- Name: t_expence_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_expence_idx ON t_expence USING btree (date);


--
-- Name: t_expence_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_expence_idx1 ON t_expence USING btree (nota);


--
-- Name: t_stock_adjustment_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_stock_adjustment_idx ON t_stock_adjustment USING btree (date);


--
-- Name: t_retur_beli_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_beli_product_idx ON t_purchase_return USING btree (date);


--
-- Name: t_retur_beli_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_beli_product_idx1 ON t_purchase_return USING btree (nota);


--
-- Name: t_retur_jual_product_idx; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_jual_product_idx ON t_sales_return USING btree (date);


--
-- Name: t_retur_jual_product_idx1; Type: INDEX; Schema: public; Owner: postgres; Tablespace: 
--

CREATE INDEX t_retur_jual_product_idx1 ON t_sales_return USING btree (nota);


--
-- Name: tr_hapus_header_ad; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_hapus_header_ad AFTER DELETE ON t_debt_payment_item FOR EACH ROW EXECUTE PROCEDURE f_delete_payable_product_header();


--
-- Name: tr_hapus_header_ad; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_hapus_header_ad AFTER DELETE ON t_credit_payment_item FOR EACH ROW EXECUTE PROCEDURE f_delete_receivable_product_header();


--
-- Name: tr_log_last_update; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_log_last_update AFTER UPDATE OF iselling_price ON m_product FOR EACH ROW EXECUTE PROCEDURE fn_log_last_update();


--
-- Name: tr_penyesuaian_stok_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_penyesuaian_stok_aiud AFTER INSERT OR DELETE OR UPDATE ON t_stock_adjustment FOR EACH ROW EXECUTE PROCEDURE f_adjust_product_stock_aiud();


--
-- Name: tr_update_return_quantity_beli; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_return_quantity_beli AFTER INSERT OR DELETE OR UPDATE ON t_purchase_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_purchase_return_quantity();


--
-- Name: tr_update_return_quantity_jual; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_return_quantity_jual AFTER INSERT OR DELETE OR UPDATE ON t_sales_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_return_quantity_sales();


--
-- Name: tr_update_pelunasan_beli_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_pelunasan_beli_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_debt_payment_item FOR EACH ROW EXECUTE PROCEDURE f_update_purchase_payment();


--
-- Name: tr_update_pelunasan_jual_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_pelunasan_jual_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_credit_payment_item FOR EACH ROW EXECUTE PROCEDURE f_update_sales_payment();


--
-- Name: tr_update_pelunasan_loan_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_pelunasan_loan_aiud AFTER INSERT OR DELETE OR UPDATE ON t_pembayaran_loan FOR EACH ROW EXECUTE PROCEDURE f_update_loan_payment();


--
-- Name: tr_update_stok_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_stok_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_purchase_order_item FOR EACH ROW EXECUTE PROCEDURE f_add_product_stock();


--
-- Name: tr_update_stok_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_stok_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_sales_order_item FOR EACH ROW EXECUTE PROCEDURE f_reduce_product_stock();


--
-- Name: tr_update_total_beli_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_beli_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_purchase_order_item FOR EACH ROW EXECUTE PROCEDURE f_update_total_purchase();


--
-- Name: tr_update_total_debt_produk_supplier; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_debt_produk_supplier AFTER INSERT OR DELETE OR UPDATE ON t_purchase_product FOR EACH ROW EXECUTE PROCEDURE f_update_supplier_total_debt();


--
-- Name: tr_update_total_jual_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_jual_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_sales_order_item FOR EACH ROW EXECUTE PROCEDURE f_update_total_sales();


--
-- Name: tr_update_total_loan_karyawan; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_loan_karyawan AFTER INSERT OR DELETE OR UPDATE ON t_loan FOR EACH ROW EXECUTE PROCEDURE f_update_employee_total_loan();


--
-- Name: tr_update_total_pengeluaran_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_pengeluaran_aiud AFTER INSERT OR DELETE OR UPDATE ON t_expense_item FOR EACH ROW EXECUTE PROCEDURE f_update_total_expenses();


--
-- Name: tr_update_total_credit_customer; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_credit_customer AFTER INSERT OR DELETE OR UPDATE ON t_product_sales FOR EACH ROW EXECUTE PROCEDURE f_update_customer_total_credit();


--
-- Name: tr_update_total_retur_beli_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_retur_beli_aiud AFTER INSERT OR DELETE OR UPDATE ON t_purchase_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_purchase_return_total_aiud();


--
-- Name: tr_update_total_retur_produk_aiud; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER tr_update_total_retur_produk_aiud AFTER INSERT OR DELETE OR UPDATE ON t_sales_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_sales_return_total_aiud();


--
-- Name: m_wholesale_price_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_wholesale_price
    ADD CONSTRAINT m_wholesale_price_fk FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_item_menu_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_item_menu
    ADD CONSTRAINT m_item_menu_fk FOREIGN KEY (menu_id) REFERENCES m_menu(menu_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_regency2_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_regency2
    ADD CONSTRAINT m_regency2_fk FOREIGN KEY (province_id) REFERENCES m_province2(province_id) ON UPDATE CASCADE;


--
-- Name: m_regency_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_regency
    ADD CONSTRAINT m_regency_fk FOREIGN KEY (province_id) REFERENCES m_province(province_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_employee_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_employee
    ADD CONSTRAINT m_employee_fk FOREIGN KEY (job_titles_id) REFERENCES m_job_titles(job_titles_id) ON UPDATE CASCADE;


--
-- Name: m_subdistrict_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_subdistrict
    ADD CONSTRAINT m_subdistrict_fk FOREIGN KEY (regency_id) REFERENCES m_regency2(regency_id) ON UPDATE CASCADE;


--
-- Name: m_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_user
    ADD CONSTRAINT m_user_fk FOREIGN KEY (role_id) REFERENCES m_role(role_id) ON UPDATE CASCADE;


--
-- Name: m_product_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_product
    ADD CONSTRAINT m_product_fk FOREIGN KEY (category_id) REFERENCES m_category(category_id) ON UPDATE CASCADE;


--
-- Name: m_role_privilege_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_role_privilege
    ADD CONSTRAINT m_role_privilege_fk FOREIGN KEY (menu_id) REFERENCES m_menu(menu_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: m_role_privilege_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY m_role_privilege
    ADD CONSTRAINT m_role_privilege_fk1 FOREIGN KEY (role_id) REFERENCES m_role(role_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_purchase_product_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_product_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_fk1 FOREIGN KEY (supplier_id) REFERENCES m_supplier(supplier_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_product_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_product
    ADD CONSTRAINT t_purchase_product_fk2 FOREIGN KEY (retur_beli_product_id) REFERENCES t_purchase_return(retur_beli_product_id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: t_bon_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_loan
    ADD CONSTRAINT t_bon_fk FOREIGN KEY (employee_id) REFERENCES m_employee(employee_id) ON UPDATE CASCADE;


--
-- Name: t_bon_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_loan
    ADD CONSTRAINT t_bon_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_employee_salary_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_employee_salary
    ADD CONSTRAINT t_employee_salary_fk FOREIGN KEY (employee_id) REFERENCES m_employee(employee_id) ON UPDATE CASCADE;


--
-- Name: t_employee_salary_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_employee_salary
    ADD CONSTRAINT t_employee_salary_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_order_item_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_fk FOREIGN KEY (beli_product_id) REFERENCES t_purchase_product(beli_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_purchase_order_item_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_order_item_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_order_item
    ADD CONSTRAINT t_purchase_order_item_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_item_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_fk FOREIGN KEY (sale_id) REFERENCES t_product_sales(sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_item_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_order_item
    ADD CONSTRAINT t_item_jual_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_debt_payment_item_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_fk FOREIGN KEY (pembayaran_hutang_product_id) REFERENCES t_product_payable_payment(pembayaran_hutang_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_debt_payment_item_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_debt_payment_item
    ADD CONSTRAINT t_debt_payment_item_fk1 FOREIGN KEY (beli_product_id) REFERENCES t_purchase_product(beli_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pembayaran_piutang_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_credit_payment_item
    ADD CONSTRAINT t_item_pembayaran_piutang_fk FOREIGN KEY (pay_sale_id) REFERENCES t_product_receivable_payment(pay_sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pembayaran_piutang_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_credit_payment_item
    ADD CONSTRAINT t_item_pembayaran_piutang_fk1 FOREIGN KEY (sale_id) REFERENCES t_product_sales(sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pengeluaran_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_fk FOREIGN KEY (expense_id) REFERENCES t_expence(expense_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_pengeluaran_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_item_pengeluaran_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expense_item
    ADD CONSTRAINT t_item_pengeluaran_fk2 FOREIGN KEY (expense_type_id) REFERENCES m_expense_type(expense_type_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_item_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk FOREIGN KEY (retur_beli_product_id) REFERENCES t_purchase_return(retur_beli_product_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_purchase_return_item_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_item_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_item_fk3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return_item
    ADD CONSTRAINT t_purchase_return_item_fk3 FOREIGN KEY (purchase_item_id) REFERENCES t_purchase_order_item(item_beli_product_id) ON UPDATE CASCADE;


--
-- Name: t_item_retur_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk FOREIGN KEY (retur_sale_id) REFERENCES t_sales_return(retur_sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_item_retur_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_item_retur_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk2 FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_item_retur_jual_fk3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return_item
    ADD CONSTRAINT t_item_retur_jual_fk3 FOREIGN KEY (item_sale_id) REFERENCES t_sales_order_item(item_sale_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk1 FOREIGN KEY (customer_id) REFERENCES m_customer(customer_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk2 FOREIGN KEY (retur_sale_id) REFERENCES t_sales_return(retur_sale_id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: t_jual_fk3; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk3 FOREIGN KEY (shift_id) REFERENCES m_shift(shift_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk4; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk4 FOREIGN KEY (dropshipper_id) REFERENCES m_dropshipper(dropshipper_id) ON UPDATE CASCADE;


--
-- Name: t_jual_fk5; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_sales
    ADD CONSTRAINT t_jual_fk5 FOREIGN KEY (card_id) REFERENCES m_card(card_id) ON UPDATE CASCADE;


--
-- Name: t_machine_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_machine
    ADD CONSTRAINT t_machine_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_bon_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_bon_fk1 FOREIGN KEY (gaji_employee_id) REFERENCES t_employee_salary(gaji_employee_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_bon_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_bon_fk2 FOREIGN KEY (loan_id) REFERENCES t_loan(loan_id) ON UPDATE CASCADE;


--
-- Name: t_product_payable_payment_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_payable_payment
    ADD CONSTRAINT t_product_payable_payment_fk FOREIGN KEY (supplier_id) REFERENCES m_supplier(supplier_id) ON UPDATE CASCADE;


--
-- Name: t_product_payable_payment_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_payable_payment
    ADD CONSTRAINT t_product_payable_payment_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_loan_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_pembayaran_loan
    ADD CONSTRAINT t_pembayaran_loan_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_piutang_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_receivable_payment
    ADD CONSTRAINT t_pembayaran_piutang_fk FOREIGN KEY (customer_id) REFERENCES m_customer(customer_id) ON UPDATE CASCADE;


--
-- Name: t_pembayaran_piutang_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_product_receivable_payment
    ADD CONSTRAINT t_pembayaran_piutang_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_pengeluaran_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_expence
    ADD CONSTRAINT t_pengeluaran_fk FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_stock_adjustment_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_stock_adjustment
    ADD CONSTRAINT t_stock_adjustment_fk FOREIGN KEY (product_id) REFERENCES m_product(product_id) ON UPDATE CASCADE;


--
-- Name: t_stock_adjustment_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_stock_adjustment
    ADD CONSTRAINT t_stock_adjustment_fk1 FOREIGN KEY (adjustment_reason_id) REFERENCES m_adjustment_reason(stock_adjustment_reason_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_fk FOREIGN KEY (beli_product_id) REFERENCES t_purchase_product(beli_product_id) ON UPDATE CASCADE ON DELETE SET NULL;


--
-- Name: t_purchase_return_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_purchase_return_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_purchase_return
    ADD CONSTRAINT t_purchase_return_fk2 FOREIGN KEY (supplier_id) REFERENCES m_supplier(supplier_id) ON UPDATE CASCADE;


--
-- Name: t_retur_jual_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_fk FOREIGN KEY (sale_id) REFERENCES t_product_sales(sale_id) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: t_retur_jual_fk1; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_fk1 FOREIGN KEY (user_id) REFERENCES m_user(user_id) ON UPDATE CASCADE;


--
-- Name: t_retur_jual_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY t_sales_return
    ADD CONSTRAINT t_retur_jual_fk2 FOREIGN KEY (customer_id) REFERENCES m_customer(customer_id) ON UPDATE CASCADE;


--
-- Name: public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE ALL ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM postgres;
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- PostgreSQL database dump complete
--

