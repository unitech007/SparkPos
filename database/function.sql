

CREATE TRIGGER tr_update_total_retur_beli_aiud AFTER INSERT OR DELETE OR UPDATE ON t_purchase_return_item FOR EACH ROW EXECUTE PROCEDURE f_update_purchase_return_total_aiud();
