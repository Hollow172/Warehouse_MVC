# Warehouse_MVC
School project of warehouse with products.
It is based on databases with products that can be added, removed or edited.
It has a concurency checking for the products:
- two or more edits of product at the same time
- editing product that is removed by another user at the same time; other interactions will happen whether the edit was first or remove
There is also a database for users that allows to create and delete accounts
And categories database that is not managed by the user. Categories have to be manually added to the database
