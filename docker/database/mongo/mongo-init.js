db.createUser({
    user: "user", 
    pwd: "user1234",
    roles: [
        {
            role: "readWrite",
            db: "CatalogDb",
        },
    ],
});

db.createCollection('Products');

db.Products.insertMany([
    {
        "Name": "Asus Laptop",
        "Category": "Computers",
        "Summary": "Summary",
        "Description": "Description",
        "ImageFile": "ImageFile",
        "Price": 54.93
    },
    {
        "Name": "HP Laptop",
        "Category": "Computers",
        "Summary": "Summary",
        "Description": "Description",
        "ImageFile": "ImageFile",
        "Price": 88.93
    }
]);

db.Products.remove({});