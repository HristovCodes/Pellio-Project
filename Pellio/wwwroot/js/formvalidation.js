const email = document.getElementById('rec');

// As per the HTML5 Specification
const emailRegExp = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

email.setAttribute("pattern", emailRegExp);


function getLoc() {
    let status = document.getElementById("status");

    function success(position) {
        const latitude = position.coords.latitude;
        const longitude = position.coords.longitude;

        status.innerHTML = latitude + " | " + longitude; 
        document.getElementById("lat").value = position.coords.latitude;
        document.getElementById("lon").value = position.coords.longitude;
        document.getElementById("idk").submit();
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