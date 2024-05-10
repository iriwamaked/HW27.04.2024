

document.addEventListener('DOMContentLoaded', () => {
    const authButton = document.getElementById("auth-button");
    //если нашли кнопку, к ней прикрепляем обработчик события
    //если мы вошли в систему, этой кнопки не будет
    if (authButton) authButton.addEventListener('click', authButtonClick);
});

function authButtonClick(){
    const authEmail = document.getElementById("auth-email");
    if (!authEmail) throw "Element '#auth-email' not found";
    const authPassword = document.getElementById("auth-password");
    if (!authPassword) throw "Element '#auth-password' not found";

    const authWarning = document.getElementById("auth-warning");
    if (!authWarning) throw "Element '#auth-warning' not found";

    const email = authEmail.value.trim(); //убираем лишние пробелы
    const password = authPassword.value; //тут не убираем пробелы, пароль может содержать пробелы

    if (email == "") {
        authWarning.classList.remove('visually-hidden');
        authWarning.innerText = "Введіть e-mail";
        return;
    }
    if (password == "") {
        authWarning.classList.remove('visually-hidden');
        authWarning.innerText = "Введіть пароль";
        return;
    }
    //Запитуємо сервіс автентифікації
    fetch(`/api/auth?email=${email}&password=${password}`)          //идет запрос в AuthController  /api/auth=>AuthController
        .then(r => r.json())    //  return new { status = $"Auth works: {email}, {password}" };
        .then(j => {            //приходит JSON (j={status ="ok/error"})
            console.log(j);
            if (j.status == "success") {
                //зміна статусу автентифікації вимагає  перезавантаження сторінки (ресурсу)
                window.location.reload();
            }
            else {
                authWarning.classList.remove('visually-hidden');
                authWarning.innerText = "Вхід відхилено, перевірте дані";
            }
        })
}

document.addEventListener('submit', e => {
    //перехватываем отправку форм (всех)
    const form = e.target;
    if (form.id == "product-form") {           //форма добавления категорий товаров
        e.preventDefault();                    //останавливаем автоматическую отправку
        //Собираем данные формы
        let formData = new FormData(form);
        fetch("/api/shop/product", {            // и переводим к асинхронный режим
            method: 'POST',
            body: formData                      //передаем данные на бэкэнд
        }).then(r => {
            console.log(r);
            if (r.status == 201) {
                window.location.reload();
            }
            else {
                r.text().then(alert);
            }
        })
    }
    if (form.id == "category-form") {           //форма добавления категорий товаров
        e.preventDefault();                    //останавливаем автоматическую отправку
        //Собираем данные формы
        let formData = new FormData(form);
        fetch("/api/shop/category", {            // и переводим к асинхронный режим
            method: 'POST',
            body: formData                      //передаем данные на бэкэнд
        }).then(r => {
            console.log(r);
            if (r.status == 201) {
                window.location.reload();
            }
            else {
                r.text().then(alert);
            }
        })
    }
})