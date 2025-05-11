// Panel geçiş fonksiyonları
function showLogin() {
    document.getElementById('loginPanel').style.display = '';
    document.getElementById('registerPanel').style.display = 'none';
}

function showRegister() {
    document.getElementById('loginPanel').style.display = 'none';
    document.getElementById('registerPanel').style.display = '';
}

function registerUser() {
    const username = document.getElementById('registerUsername').value.trim();
    const password = document.getElementById('registerPassword').value;
    const password2 = document.getElementById('registerPassword2').value;
    const errorDiv = document.getElementById('registerError');
    errorDiv.textContent = '';

    if (!username || !password || !password2) {
        errorDiv.textContent = 'Tüm alanları doldurun.';
        return;
    }
    if (password !== password2) {
        errorDiv.textContent = 'Şifreler uyuşmuyor.';
        return;
    }
    fetch('/api/user/all')
    .then(res => res.json())
    .then(users => {
        if (users.includes(username)) {
            errorDiv.textContent = 'Bu kullanıcı adı zaten kayıtlı.';
            return;
        }

        // Eğer kullanıcı adı alınmamışsa kayıt işlemine devam et
        return fetch('/api/user/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ username, password })
        })
        .then(response => {
            if (!response.ok) {
                return response.text().then(text => { throw new Error(text); });
            }
            return response.text();
        })
        .then(message => {
            alert(message);
            showLogin();
        });
    })
    .catch(err => {
        errorDiv.textContent = err.message;
    });
}

// Giriş fonksiyonu (hem kullanıcı hem admin için)
function login() {
    const username = document.getElementById('loginUsername').value.trim();
    const password = document.getElementById('loginPassword').value;
    const errorDiv = document.getElementById('loginError');
    errorDiv.textContent = '';

    // Normal kullanıcı kontrolü
    // Backend'e giriş isteği gönder
    fetch('/api/user/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password })
    })
    .then(response => {
        if (!response.ok) {
            return response.text().then(text => { throw new Error(text); });
        }
        return response.text();
    })
    .then(role => {
        console.log(role);
        if(role === "Admin")
        {
            window.location.href = '/admin';
        }
        else if(role == "User")
        {
            window.location.href = '/home';
        }
    })
    .catch(err => {
        errorDiv.textContent = err.message;
    });
}