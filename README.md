# Софтуерна Документация
# ⬛Pellio⬛
# Отбор:
# Ивайло Христов - Дамян Атанасов -  Божидар Атанасов

# Какво е Pellio?
“Pellio” е платформа за поръчване на храна онлайн и сайт, който свързва потребителите си с нашия ресторант. В “Pellio” ние вярваме, че поръчването на храна онлайн трябва да бъде лесно и без напрежение. С няколко клика можеш да поръчаш от огромното разнообразие на различни типове кухня онлайн. “Pellio” ти осигурява голям избор от храни, на достъпни цени.
Продуктите, които предлагаме ще бъдат представени на главната страница, на едно място, за удобство на потребителя. При натискане бутона за поръчване ще отиде на друга страница показваща името, съставките, цената, ревюта плюс бутон за добавяне в количката. След като потребителя си избере желаните ястия, натиска бутона горе в дясно и може да види кошницата си, продуктите в нея и да завърши поръчката. За да завърши поръчката трябва да бъде въведно - адрес за доставка, телефонен номер, име и e-mail адрес.

# Нужни познания за работа по Pellio:
За да може един разработчик да разбира и работи по приложението “Pellio” трябва да бъде запознат със следните технологии - C#;  .NET;  EF;  ES6;  HTML5;  CSS3; MVC; MSSQLSERVER и като платформа Visual Studio 2019.

# Пускане на проекта:
- Изтегляне на .NET Core 3.1 SDK или по-късна версия;  
- Отвраряне на проекта във Visual Studio 2019 или по ново след клониране;  
- Приемане на HTTPS сертификата;  
- Пускане с CTRL + F5;  
- Навигиране до ip-то показано в конзолата, ако браузъра не се отвори сам;  

# Използвани библиотеки:
A .Net Wrapper for The OpenCage Geocoder - https://github.com/gingemonster/dotnet-opencage-geocode

# Конвенвенциите които спазваме са тук: 
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
        // GET: Products
        /// <summary>
        /// Acts as a Main function. Makes call to uuidc create function.
        /// </summary>
        /// <returns>Displays all products from db.</returns>
        [Route("")]
        [Route("Products")]
        [Route("Products/Index")]
        public async Task<IActionResult> Index(string TagsDropdown)
        {
            var creds = new EmailCredentials();
            creds.Email = "fokenlasersights@gmail.com";
            creds.Password = "******";
            _context.Add(creds);
            await _context.SaveChangesAsync();

            FillDropDownTags();
            GenUUIDC();
            if (TagsDropdown == null || TagsDropdown == "Всички")
            {
                return View(await _context.Products.ToListAsync());
            }
            else
            {
                return View(await _context.Products.Where(p => p.Tag == TagsDropdown).ToListAsync());
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
> Първо проверяваме дали Id е null и дали съществува Product. Ако няма Лист с коментари се създава такъв и се пълни с всички коментари споделящи id с продукта.Ако има лист но не и коментари се извежда такова съобщение. Ако ли не се извежда средната оценка. Използваики ProductComment модела се извеждат коментарите и данни за продукта на едно View.

## OrdersListsController.cs - Съдържа функции работещи с количката обекти, база данни и визуални елементи (.cshtml)

```csharp
        // GET: OrdersLists
        public async Task<IActionResult> Index()
        {
            OrderListCleanUp();

            string uid = Request.Cookies["uuidc"];

            var cart = await _context.OrdersList
                .Include(c => c.Products).FirstOrDefaultAsync(m => m.UserId == uid);

            if (cart == null)
            {
                cart = new OrdersList
                {
                    Total = 0,
                    UserId = uid,
                    TimeMade = DateTime.Now.ToString("MM/dd/yyyy"),
                    Products = new List<Products>()
                };
                _context.Add(cart);
                await _context.SaveChangesAsync();
            }


            OrderListMadeOrder combo = new OrderListMadeOrder
            {
                OrdersList = cart,
                MadeOrder = _context.MadeOrder.Where(mo => mo.UserId == uid).ToList()
            };

            return View(combo);
        }
```
> OrderListCleanUp функция бива повикана да изчисти стари ентрита в базата. Колекция от продукти притежавани от това определено uuidc бива извадена от база с данни. Ако няма такава се създава. Използва се OrderListMadeOrder модела за да се покаже на един .cshtml както текущата количка така и предишни завършени поръчки от uuidc.


 ```csharp
        /// <summary>
        /// Sends the email via gmail smtp server
        /// </summary>
        /// <param name="rec">short for reciver</param>
        /// <param name="mes">short for messege</param>
        async public Task SendMail(string rec, string mes)
        {
            string uid = Request.Cookies["uuidc"];
            try
            {
                if (ModelState.IsValid)
                {
                    var completed = _context.MadeOrder.Where(c => c.UserId == uid).Count();
                    if (completed % 3 == 0)
                    {
                        var code = new PercentOffCode
                        {
                            Code = CodeGenerate(),
                            Percentage = 5,
                            Available = true
                        };
                        _context.PercentOffCodes.Add(code);
                        var credsfromdb = _context.EmailCredentials.FirstOrDefault();
                        var client = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential(credsfromdb.Email, credsfromdb.Password),
                            EnableSsl = true
                        };
                        mes = mes.TrimEnd(',');
                        client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods пможе да получи намаление с код " + code.Code + ", направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                    }
                    else
                    {
                        var credsfromdb = _context.EmailCredentials.FirstOrDefault();
                        var client = new SmtpClient("smtp.gmail.com", 587)
                        {

                            Credentials = new NetworkCredential(credsfromdb.Email, credsfromdb.Password),
                            EnableSsl = true
                        };
                        mes = mes.TrimEnd(',');
                        client.Send("fokenlasersights@gmail.com", rec, "Вашата покупка от Pellio-Foods направена на " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), mes.TrimEnd(','));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
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
