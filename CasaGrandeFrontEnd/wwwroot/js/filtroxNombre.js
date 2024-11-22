document.getElementById("searchBox").addEventListener("keyup", function () {
    var filter = this.value.toLowerCase();
    var users = document.querySelectorAll("#userList .tarjetaUser");

    users.forEach(function (user) {
        var name = user.querySelector("p strong").nextSibling.textContent.toLowerCase();
        if (name.includes(filter)) {
            user.style.display = "";
        } else {
            user.style.display = "none";
        }
    });
});
