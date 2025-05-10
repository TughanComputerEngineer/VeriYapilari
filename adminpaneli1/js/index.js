// Panel geçiş fonksiyonları
function showLogin() {
    document.getElementById('loginPanel').style.display = '';
    document.getElementById('registerPanel').style.display = 'none';
}

function showRegister() {
    document.getElementById('loginPanel').style.display = 'none';
    document.getElementById('registerPanel').style.display = '';
}

// Kullanıcı kayıt
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
    let users = JSON.parse(localStorage.getItem('users') || '[]');
    if (users.find(u => u.username === username)) {
        errorDiv.textContent = 'Bu kullanıcı adı zaten kayıtlı.';
        return;
    }
    users.push({ username, password });
    localStorage.setItem('users', JSON.stringify(users));
    alert('Kayıt başarılı! Giriş yapabilirsiniz.');
    showLogin();
}

// Giriş fonksiyonu (hem kullanıcı hem admin için)
function login() {
    const username = document.getElementById('loginUsername').value.trim();
    const password = document.getElementById('loginPassword').value;
    const errorDiv = document.getElementById('loginError');
    errorDiv.textContent = '';

    // Admin kontrolü
    if (username === 'admin' && password === 'admin123') {
        window.location.href = 'admin.html';
        return;
    }

    // Normal kullanıcı kontrolü
    let users = JSON.parse(localStorage.getItem('users') || '[]');
    if (users.find(u => u.username === username && u.password === password)) {
        // Giriş başarılı, anasayfaya yönlendir (şimdilik alert)
        alert('Giriş başarılı! (Anasayfa daha sonra eklenecek)');
    } else {
        errorDiv.textContent = 'Kullanıcı adı veya şifre hatalı.';
    }
} 