// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const burgeropen = document.getElementById("burgeropen");
const burgerclose = document.getElementById("burgerclose");
const menu = document.getElementById("menu");

burgeropen.addEventListener("click", () => {
    menu.classList.add("menu")
})

burgerclose.addEventListener("click", () => {
    menu.classList.remove("menu");
})