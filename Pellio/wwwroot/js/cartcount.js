let cart = document.getElementById("cart");

cart.innerHTML = getcookie(" cartitems") + " | Кошница ";

function getcookie(cookiename) {
    let cookies = document.cookie;
    let allcookies = cookies.split(";");

    for (let i = 0; i < allcookies.length; i++) {
        name = allcookies[i].split('=')[0];
        value = allcookies[i].split('=')[1];
        if (name === cookiename) {
            return value;
        }
    }
    return "0";
}

window.onload = genUUIDcookie();


function genUUIDcookie() {
    if (document.cookie.indexOf('uuidc=') == -1) {
        console.log("Loaded.");
        let expdate = new Date();
        expdate.setMonth(expdate.getMonth() + 1);
        expdate = expdate.toUTCString();

        document.cookie = "uuidc =" + uuidv4() + ";" + "expires =" + expdate + ";";
    }   
}

function uuidv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}