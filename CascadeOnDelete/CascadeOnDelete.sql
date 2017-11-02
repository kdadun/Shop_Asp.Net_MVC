Alter Table Products
Add constraint fk_category_Id
 FOREIGN KEY (CategoryId)
    REFERENCES Categories (Id)
    ON DELETE CASCADE