# [Bulgarian](https://github.com/HristovCodes/Pellio-Project#bulgarian)/[English](https://github.com/HristovCodes/Pellio-Project/blob/master/README.md#english)
# English:
# Software Documentation
# Team:
Ivailo Hristov - Damian Atanasov - Bojidar Atanasov

# What is Pellio?
"Pellio is a platform for ordering food online and a website that allows users to connect with our restaraunt. In "Pellio" we believe that ordering food online needs to be easy and without any complications. With a few clicks you can order anything from our huge variety of different cuisines. "Pellio" guarantees you a broad range of food choices all on reasonable prices. The products we offer will be displayed on the home page, all in one place, for the ease of the user. Upon clicking the button for ordering you will be redirected to a page consisting of the product's name, ingredients, price, reviews and a button to add the product to your cart. After you, the user, have chosen your desired dishes you can click the cart button in the top right corner which will take you to the order page where you can finalize your order and see the products in your cart. To complete the order one must input their adress, phone number, name and e-mail adress.

# Skills required to work on Pellio:
For a develepor to be able to work on "Pellio" he would need to be familiar with the following technologies:
- C#
- .Net
- EF
- ES6
- HTML5
- CC3
- MSSQLServer
- MVC

# Building and running the project
- Install .Net Core 3.1 SDK or later
- Install Asp.Net Core 3.1 Framework or later
- Make sure that Asp.Net and web develepment for VS2019 is installed from Visual Studio Installer
- Accept the HTTPS certificate upon launch of the project
- Press CTRL + F5
- Navigate to local host if your browser doesn't do it automatically
- Press the "FillDB" button on the main page to fill the database with test data.

# Libraries used
- A .Net Wrapper for The OpenCage Geocoder - https://github.com/gingemonster/dotnet-opencage-geocode

# Code style conventions
- The project follows the official code style conventions used by Microsoft which be found [here](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).

# User requirements
- a way for communication including phone and e-mail
- main characteristics of the good/services offered
- explanation on the use of cookies and other ways of saving user data
- working cart
- all products should be visible on the main page
- each products should have a description
- price should be displayed before and after purchase
- discount codes should be given out for the first purchase and every third one after that
- after agreeing to it the user should be able to fill in their adress automatically by IP
- about section for the restaraunt
- different categories that allow sorting by the specified criteria
- payment in cash and in bulgarian currency

# UI
![Image of website](https://i.imgur.com/SUsr29e.png)

Cart - Pressing it will take you the cart page where you edit and finalize your order.

Navigation (menu) - Dropdown menu with links to Contacts, About and Cart.

Contacts - Here you can find FaQ's and ways to get in contact with us.

About - Information about the restaraunt.

Order - Pressing it it will take you to a page where you can order the product and see information about it like ingredients, price and also leave and see reviews.

# Class Diagram
![Image of class diagram](https://github.com/HristovCodes/Pellio-Project/blob/master/Pellio/patternMatchingClassDiagram.png)

# Entity Relationship Diagram (DB Diagram)
![Image of er diagram](https://i.imgur.com/CZJT6Nl.png)

# Explanation of the more important classes and their methods
## ProductsController.cs - Contains methods for working with products, the databse and visual elements (.cshtml)


```csharp
        /// GET: Products
        /// <summary>
        /// Acts as a Main function. Makes call to uuidc create function.
        /// </summary>
        /// <param name="categories">Categories for text in buttons for sorting.</param>
        /// <returns>Displays all products from db.</returns>
        [Route("")]
        [Route("Products")]
        [Route("Products/Index")]
        public async Task<IActionResult> Index(string TagsDropdown)
        {
            _context.SaveChanges();
            FillDropDownTags();
            
            string uid = Request.Cookies["uuidc"];

            if (_context.OrdersList
                    .Include(c => c.Products).FirstOrDefault(m => m.UserId == uid) == null)
            {
                _context.OrdersList.Add(new OrdersList
                {
                    Products = new List<Products>(),
                    Total = 0,
                    UserId = uid,
                    PercentOffCode = new PercentOffCode()
                    {
                        Code = "todd",
                        Percentage = 0,
                        Usable = false
                    }
                });
                _context.SaveChanges();
            }

            if (TagsDropdown == null || TagsDropdown == "Всички")
            {
                return View(await _context.Products.ToListAsync());
            }
            else
            {
                return View(await _context.Products.Where(p => p.Tag == TagsDropdown).ToListAsync());
            }
        }
    }
}
```
> Acts as a home page. Upon loading the web app this is the first thing you see. TagsDropdown - used for sorting and is used by FillDropDownTags(). On load it is null, afterwards it receives a value from an event in Index.cshtml. Returns a view Index.cshtml sorter/unsorted with a list of all the products in the database.


```csharp
        // GET: Products/Order/5
        /// <summary>
        /// Returns a view of the chosen products (by id) with additional information about it and comments.
        /// </summary>
        /// <returns>View with both Product data and comments for Product</returns>
        public async Task<IActionResult> Order(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.Include(co => co.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            if (products.Comments == null)//check if it is null
            {
                products.Comments = new List<Comments>();
                foreach (var com in _context.Comments)
                {
                    if (!products.Comments.Contains(com))
                    {
                        if (com.ProductsId == products.Id)
                        {
                            products.Comments.Add(com);
                        }
                    }
                }
            }

            if (!products.Comments.Any())//check if any comments connected to product
            {//if not tell user there is no score
                ViewBag.avg_score = "За съжаление този продукт все още няма потребителски оценки. Можете да помогнете да промените това!";
            }
            else
            {
                //if avarage score and add to viewbag
                var avg_score = products.Comments.Average(sc => sc.Score);
                var rounded = Math.Round(avg_score, 2);
                ViewBag.avg_score = "Нашите потребители средно дават на това ястие оценката: " + rounded;
            }

            ProductComment productComment = new ProductComment()
            {
                Products = products,
                Comments = new Comments()
            };

            return View(productComment);
        }
```
> We check if Id is null and if it contains Product. If it doesn't have a List with comments we create one which is then filled with comments sharing the product's id. If there's a list but no comments a message is displayed notifying the user there's no reviews yet. Finally if everything is fine an average score is calculated and displayed including all the comments from the ProductComment model. All is returned in one view (info, reviews, score).


 ```csharp
        /// <summary>
        /// Sends the email via gmail Smtp server.
        /// </summary>
        /// <param name="rec">Short for reciver.</param>
        /// <param name="mes">Short for messege.</param>
        public void SendMail(string rec)
        {
            string uid = Request.Cookies["uuidc"];
            string mes = GenEmailMsg();
            var codebruh = _context.OrdersList.Include(c => c.PercentOffCode)
                .Where(a => a.UserId == uid).First().PercentOffCode.Code;
            if (ModelState.IsValid)
            {
                var completed = _context.MadeOrder.Where(c => c.UserId == uid).Count();
                if (completed % 3 == 0)
                {
                    var code = new PercentOffCode
                    {
                        Code = CodeGenerate(),
                        Percentage = 5,
                        Usable = true
                    };
                    _context.PercentOffCodes.Add(code);
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_appSettings.Email_name, _appSettings.Email_pass),
                        EnableSsl = true
                    };
                    //mes = mes.TrimEnd(',');
                    //mes = mes.Replace("&", "\n");
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods може да получи намаление с код " + code.Code + ", направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                }
                else
                {
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_appSettings.Email_name, _appSettings.Email_pass),
                        EnableSsl = true
                    };

                    mes = mes.TrimEnd(',');
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                }
            }
        }
```  
> The SendMail method is called when the user orders. For the first or every third purchase for any given uuidc a discount code (5%) is sent to the user which can be used once. For non-discount receiving orders a normal message is sent without a discount code.


 ```csharp
        public static string CodeGenerate()

        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
```  
> CodeGenerate generates the code used in SendMail which is sent to the user so they can use their discount. It consists of 8 random characters. [A-Za-z0-9]


# Bulgarian:
# Софтуерна Документация
# ⬛Pellio⬛ 
# Отбор:
Ивайло Христов - Дамян Атанасов -  Божидар Атанасов

# Какво е Pellio?
“Pellio” е платформа за поръчване на храна онлайн и сайт, който свързва потребителите си с нашия ресторант. В “Pellio” ние вярваме, че поръчването на храна онлайн трябва да бъде лесно и без напрежение. С няколко клика можеш да поръчаш от огромното разнообразие на различни типове кухня онлайн. “Pellio” ти осигурява голям избор от храни, на достъпни цени.
Продуктите, които предлагаме ще бъдат представени на главната страница, на едно място, за удобство на потребителя. При натискане бутона за поръчване ще отиде на друга страница показваща името, съставките, цената, ревюта плюс бутон за добавяне в количката. След като потребителя си избере желаните ястия, натиска бутона горе в дясно и може да види кошницата си, продуктите в нея и да завърши поръчката. За да завърши поръчката трябва да бъде във едно - адрес за доставка, телефонен номер, име и e-mail адрес.

# Нужни познания за работа по Pellio:
За да може един разработчик да разбира и работи по приложението “Pellio” трябва да бъде запознат със следните технологии - C#;  .NET;  EF;  ES6;  HTML5;  CSS3; MVC; MSSQLSERVER и като платформа Visual Studio 2019.

# Пускане на проекта:
- Изтегляне на .NET Core 3.1 SDK или по-късна версия;  
- Изтегляне на Asp.Net Core 3.1 Framework или по-късна версия; 
- Проверете дали имате ASP.NET and web development за Visual Studio 2019 инсталирано от Visual Studio Installer;
- Отваряне на проекта във Visual Studio 2019 или по ново след клониране;  
- Приемане на HTTPS сертификата;  
- Пускане с CTRL + F5;  
- Навигиране до ip-то показано в конзолата, ако браузъра не се отвори сам;  
- Натискане на бутона "FillDB" с цел напълване на базата с тестови данни;

# Използвани библиотеки:
A .Net Wrapper for The OpenCage Geocoder - https://github.com/gingemonster/dotnet-opencage-geocode

# Конвенциите които спазваме са тук:
https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions


# Потребителски изисквания:  
- име/фирма на оператора на сайта;  
- данни за кореспонденция, включително телефон и адрес на електронна поща;  
- основните характеристики на стоките или услугите;  
- отговорност на оператора на сайта за плащане, доставка, изпълнение;  
- отговорности на оператора на сайта:  

     - за действията на определени лица (например собствени служители).  
     - според стойността на вредата (да не се носи отговорност за незначителни вреди)  

- указания за използване на бисквитки и други средства за краткотрайно запаметяване на данни на потребителите;  
- доказателства – копия от платежен документ (касова бележка или фактура);  
- да има работеща кошница;  
- да се виждат продуктите в главното меню;  
- да се изписва съдържанието на продукта;  
- да се вижда цената преди и след поръчка;  
- кодовете за отстъпка да работят на всяка първа и трета следваща поръчка;  
- при съгласяване от потребител да се намери приблизителният адрес от IP;
- да има номер за обратна връзка;  
- да има информация относно ресторанта;  
- да има категории с който да се сортират продуктите по зададения критерии;  
- плащането да става в брой и в българска валута;  

# Потребителски интерфейс:
![Image of website](https://i.imgur.com/SUsr29e.png)

Кошница - При натискане ще те отведе към кошницата ти от която можеш да персонализираш поръчката си;

Навигация (меню) - Падащо меню с препратки към Контакти, За нас и Кошница;

Контакти - Тук може да намерите често задавани въпроси и контакти за връзка с нас;

За нас - Информация относно ресторанта;

Поръчай - При натискане те отвежда в друга страница в която можеш да поръчаш, видиш съставки и да напишеш или видиш коментар;

# Клас диаграма:
![Image of class diagram](https://github.com/HristovCodes/Pellio-Project/blob/master/Pellio/patternMatchingClassDiagram.png)
# Диаграма на базите данни:
![Image of er diagram](https://i.imgur.com/CZJT6Nl.png)

# Описване на по-важни класове и методи:

## ProductsController.cs - Съдържа функции работещи с продукти обекти, база данни и визуални елементи (.cshtml)

```csharp
        /// GET: Products
        /// <summary>
        /// Acts as a Main function. Makes call to uuidc create function.
        /// </summary>
        /// <param name="categories">Categories for text in buttons for sorting.</param>
        /// <returns>Displays all products from db.</returns>
        [Route("")]
        [Route("Products")]
        [Route("Products/Index")]
        public async Task<IActionResult> Index(string TagsDropdown)
        {
            _context.SaveChanges();
            FillDropDownTags();
            
            string uid = Request.Cookies["uuidc"];

            if (_context.OrdersList
                    .Include(c => c.Products).FirstOrDefault(m => m.UserId == uid) == null)
            {
                _context.OrdersList.Add(new OrdersList
                {
                    Products = new List<Products>(),
                    Total = 0,
                    UserId = uid,
                    PercentOffCode = new PercentOffCode()
                    {
                        Code = "todd",
                        Percentage = 0,
                        Usable = false
                    }
                });
                _context.SaveChanges();
            }

            if (TagsDropdown == null || TagsDropdown == "Всички")
            {
                return View(await _context.Products.ToListAsync());
            }
            else
            {
                return View(await _context.Products.Where(p => p.Tag == TagsDropdown).ToListAsync());
            }
        }
    }
}
```
> Играе ролята на главна страница. При зареждане на WebApp това е първата страница която зарежда. TagsDropdown - използва се за сортиране и се получава от FillDropDownTags(). При първо зареждане е null, после получава стойност от Index.cshtml event. Поставя на Index.cshtml сортиран или не сортиран лист от продукти от база данни.


```csharp
        // GET: Products/Order/5
        /// <summary>
        /// Returns a view of the chosen products (by id) with additional information about it and comments.
        /// </summary>
        /// <returns>View with both Product data and comments for Product</returns>
        public async Task<IActionResult> Order(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.Include(co => co.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }

            if (products.Comments == null)//check if it is null
            {
                products.Comments = new List<Comments>();
                foreach (var com in _context.Comments)
                {
                    if (!products.Comments.Contains(com))
                    {
                        if (com.ProductsId == products.Id)
                        {
                            products.Comments.Add(com);
                        }
                    }
                }
            }

            if (!products.Comments.Any())//check if any comments connected to product
            {//if not tell user there is no score
                ViewBag.avg_score = "За съжаление този продукт все още няма потребителски оценки. Можете да помогнете да промените това!";
            }
            else
            {
                //if avarage score and add to viewbag
                var avg_score = products.Comments.Average(sc => sc.Score);
                var rounded = Math.Round(avg_score, 2);
                ViewBag.avg_score = "Нашите потребители средно дават на това ястие оценката: " + rounded;
            }

            ProductComment productComment = new ProductComment()
            {
                Products = products,
                Comments = new Comments()
            };

            return View(productComment);
        }
```
> Първо проверяваме дали Id е null и дали съществува Product. Ако няма Лист с коментари се създава такъв и се пълни с всички коментари споделящи id с продукта.Ако има лист но не и коментари се извежда такова съобщение. Ако ли не се извежда средната оценка. Използвайки ProductComment модела се извеждат коментарите и данни за продукта на едно View.


 ```csharp
        /// <summary>
        /// Sends the email via gmail Smtp server.
        /// </summary>
        /// <param name="rec">Short for reciver.</param>
        /// <param name="mes">Short for messege.</param>
        public void SendMail(string rec)
        {
            string uid = Request.Cookies["uuidc"];
            string mes = GenEmailMsg();
            var codebruh = _context.OrdersList.Include(c => c.PercentOffCode)
                .Where(a => a.UserId == uid).First().PercentOffCode.Code;
            if (ModelState.IsValid)
            {
                var completed = _context.MadeOrder.Where(c => c.UserId == uid).Count();
                if (completed % 3 == 0)
                {
                    var code = new PercentOffCode
                    {
                        Code = CodeGenerate(),
                        Percentage = 5,
                        Usable = true
                    };
                    _context.PercentOffCodes.Add(code);
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_appSettings.Email_name, _appSettings.Email_pass),
                        EnableSsl = true
                    };
                    //mes = mes.TrimEnd(',');
                    //mes = mes.Replace("&", "\n");
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods може да получи намаление с код " + code.Code + ", направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                }
                else
                {
                    var client = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential(_appSettings.Email_name, _appSettings.Email_pass),
                        EnableSsl = true
                    };

                    mes = mes.TrimEnd(',');
                    client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                }
            }
        }
```  
> SendMail функцията бива извиквана когато потребителят поръчва избраните от него продукти. На първа и всяка следваща трета поръчка на определеното uuidc, се изпраща код с отстъпка от 5%, който може да се използва един път и се пази в базата в случай че искат да го използват за друга поръчка. За останалите поръчки се изпраща друго съобщение без кода за отстъпка.


 ```csharp
        public static string CodeGenerate()

        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
```  
> CodeGenerate генерира този код, използван в функцията SendMail, който се изпраща на определеното uuidc, за да използва своето намаление. Той се състой от 8 случайни знака.

Github link - https://github.com/HristovCodes/Pellio-Project
