document.querySelector('#headingOne button').addEventListener('click', function () {
    var arrowIcon = document.getElementById('arrowIcon1');
    arrowIcon.classList.toggle('rotate-90');
});
document.querySelector('#headingSeven button').addEventListener('click', function () {
    var arrowIcon = document.getElementById('arrowIcon7');
    arrowIcon.classList.toggle('rotate-90');
});
document.querySelector('#headingTwo button').addEventListener('click', function () {
    var arrowIcon = document.getElementById('arrowIcon2');
    arrowIcon.classList.toggle('rotate-90');
});
document.querySelector('#headingTre button').addEventListener('click', function () {
    var arrowIcon = document.getElementById('arrowIcon3');
    arrowIcon.classList.toggle('rotate-90');
});
document.querySelector('#headingFor button').addEventListener('click', function () {
    var arrowIcon = document.getElementById('arrowIcon4');
    arrowIcon.classList.toggle('rotate-90');
});
document.querySelector('#headingFive button').addEventListener('click', function () {
    var arrowIcon = document.getElementById('arrowIcon5');
    arrowIcon.classList.toggle('rotate-90');
});
document.querySelector('#headingSix button').addEventListener('click', function () {
    var arrowIcon = document.getElementById('arrowIcon6');
    arrowIcon.classList.toggle('rotate-90');
});


document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const searchResults = document.getElementById('searchResults');

    searchInput.addEventListener('input', function () {
        const query = searchInput.value;
        if (query.length > 0) {
            fetch(`/Home/Search?query=${query}`)
                .then(response => response.json())
                .then(data => {
                    searchResults.innerHTML = '';
                    data.forEach(function (functionItem) {
                        const resultItem = document.createElement('a');
                        resultItem.href = functionItem.url;
                        resultItem.className = 'list-group-item list-group-item-action';
                        resultItem.textContent = functionItem.name;
                        searchResults.appendChild(resultItem);
                    });
                });
        } else {
            searchResults.innerHTML = '';
        }
    });
});
