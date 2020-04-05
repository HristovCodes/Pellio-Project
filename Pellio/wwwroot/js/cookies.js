const cookieswindow = document.getElementById("cookieswindow");
const acceptcookies = document.getElementById("cookiesagree");

if (document.cookie.includes("accepted=true")) {
    cookieswindow.className = "hide";
} else {
    acceptcookies.className = "window";
    acceptcookies.addEventListener("click", () => {
        cookieswindow.className = "hide";
        document.cookie = "accepted=true";
    });
}