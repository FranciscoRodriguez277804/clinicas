document.getElementById("searchBox").addEventListener("keyup", function () {
    var searchQuery = this.value.trim();

    if (searchQuery === "") {
        // Si la barra está vacía, recargar la primera página de usuarios
        fetch(`/Usuarios/ListadoDeUsuariosPartial?page=1`)
            .then(response => response.text())
            .then(data => {
                document.getElementById("userList").innerHTML = data;
            })
            .catch(error => console.error('Error al cargar la lista inicial:', error));
    } else {
        // Si hay búsqueda, cargar los resultados paginados
        fetch(`/Usuarios/Buscar?query=${encodeURIComponent(searchQuery)}&page=1`)
            .then(response => response.text())
            .then(data => {
                document.getElementById("userList").innerHTML = data;
            })
            .catch(error => console.error('Error al buscar usuarios:', error));
    }
});

document.querySelectorAll('.ver-detalles').forEach(function (button) {
    button.addEventListener('click', function () {
        var userId = this.getAttribute('data-id');

        fetch(`/Usuarios/Detalles?id=${userId}`)
            .then(response => response.text())
            .then(data => {
                var modalOverlay = document.createElement('div');
                modalOverlay.innerHTML = data;
                document.body.appendChild(modalOverlay);

                // Añadir clase para desenfoque
                document.body.classList.add('modal-open');

                // Cerrar modal
                modalOverlay.querySelector('#closeModal').addEventListener('click', function () {
                    modalOverlay.remove();
                    document.body.classList.remove('modal-open');
                });
            })
            .catch(error => {
                console.error('Error:', error);
            });
    });
});



document.querySelectorAll('.imagenUser').forEach(img => {
    // Si la imagen es la generica, la mostramos directamente y ocultamos el GIF de carga
    if (img.src.includes('foto-perfil-generica.jpg')) {
        img.style.display = 'block'; // Muestra inmediatamente la imagen genérica
        return; // Salimos del ciclo para esta imagen
    }

    img.onload = function () {
        // Ocultar el GIF de carga cuando la imagen se ha cargado
        const loadingGif = this.parentNode.querySelector('.loading-gif');
        this.style.display = 'block';        // Mostrar la imagen
    };

    // Establecer el estilo inicial para que la imagen esté oculta
    img.style.display = 'none';
});