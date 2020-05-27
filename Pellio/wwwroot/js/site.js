const burger = document.getElementById("burger");
const menu = document.getElementById("menu");

burger.addEventListener("click", () => {
    if (!menu.classList.contains("menu"))
        menu.classList.add("menu")
    else
        menu.classList.remove("menu");
})