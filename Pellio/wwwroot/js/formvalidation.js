const email = document.getElementById('rec');

// As per the HTML5 Specification
const emailRegExp = "^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

email.setAttribute("pattern", emailRegExp);