// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const burger = document.getElementById("burger");
const menu = document.getElementById("menu");

burger.addEventListener("click", () => {
    if (!menu.classList.contains("menu"))
        menu.classList.add("menu")
    else
        menu.classList.remove("menu");
})


const email = document.getElementById('rec');

// As per the HTML5 Specification
const emailRegExp = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

email.setAttribute("pattern", emailRegExp);