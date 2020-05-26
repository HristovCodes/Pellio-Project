const email = document.getElementById('rec');

// As per the HTML5 Specification
const emailRegExp = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

email.setAttribute("pattern", emailRegExp);

window.onload = CheckAddressValue();
function CheckAddressValue() {
    if (document.getElementById("address").value == "") {
        getLoc();
    }//execs getLoc() on page load if the value in address in empty
    //It is on load but if you call an on load function that reloads the page you brick that page
    //Simple way to avoid that
}


function getLoc() {
    let status = document.getElementById("status");

    function success(position) {
        const latitude = position.coords.latitude;
        const longitude = position.coords.longitude;

        status.innerHTML = latitude + " | " + longitude; 
        document.getElementById("lat").value = position.coords.latitude;
        document.getElementById("lon").value = position.coords.longitude;
        //update vals in hidden form (Is there a better way to get values to the controller?)
        //couldn't find one
        document.getElementById("address_gen").submit();//submits the hidden form
    }

    function error() {
        status.textContent = 'Unable to retrieve your location';
    }

    if (!navigator.geolocation) {
        alert('Geolocation is not supported by your browser');
    } else {
        status.innerHTML = 'Locating…';
        navigator.geolocation.getCurrentPosition(success, error);
    }
    
}