let cart = document.getElementById("cart");

cart.innerHTML = getcookie() + " | Кошница ";

function getcookie() {
    let cookies = document.cookie;
    let allcookies = cookies.split(";");

    for (let i = 0; i < allcookies.length; i++) {
        name = allcookies[i].split('=')[0];
        value = allcookies[i].split('=')[1];
        if (name === " cartitems") {
            return value;
        }
    }
    return "0";
}