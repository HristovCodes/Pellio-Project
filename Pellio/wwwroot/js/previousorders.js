let closewindow = document.getElementById("closewindow");

closewindow.addEventListener("click", () => {
    if (!isMouseInside)
        closewindow.className = "hide";
})

let openwindow = document.getElementById("openwindow");

openwindow.addEventListener("click", () => {
    closewindow.className = "prvspurchases";
})

let purchaseswindow = document.getElementById("window");
let isMouseInside = false;

purchaseswindow.addEventListener("mouseover", () => { isMouseInside = true; })
purchaseswindow.addEventListener("mouseleave", () => { isMouseInside = false; })