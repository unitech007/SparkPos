

ALTER TABLE t_delivery_items
ADD COLUMN user_id t_guid;



ALTER TABLE t_delivery_items
ADD CONSTRAINT fk_delivery_items_user_id
FOREIGN KEY (user_id)
REFERENCES m_user(user_id);