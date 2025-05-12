document.addEventListener('DOMContentLoaded', function () {
    // Logo seçme butonu
    const selectLogoBtn = document.getElementById('selectLogoBtn');
    const teamLogo = document.getElementById('teamLogo');
    const logoPreviewImg = document.getElementById('logoPreviewImg');
    const logoPlaceholder = document.querySelector('.logo-placeholder');

    if (selectLogoBtn) {
        selectLogoBtn.addEventListener('click', function () {
            teamLogo.click();
        });
    }

    if (teamLogo) {
        teamLogo.addEventListener('change', function () {
            if (this.files && this.files[0]) {
                const reader = new FileReader();

                reader.onload = function (e) {
                    logoPreviewImg.src = e.target.result;
                    logoPreviewImg.style.display = 'block';
                    logoPlaceholder.style.display = 'none';
                }

                reader.readAsDataURL(this.files[0]);
            }
        });
    }

    // Yeni takım ekleme modalını aç
    const createTeamBtn = document.getElementById('createTeamBtn');
    const teamModal = document.getElementById('teamModal');
    const closeButtons = document.querySelectorAll('.close, #cancelBtn');

    if (createTeamBtn) {
        createTeamBtn.addEventListener('click', function () {
            document.getElementById('modalTitle').textContent = 'Yeni Takım Ekle';
            document.getElementById('teamId').value = '0';
            document.getElementById('teamForm').reset();

            // Logo önizlemeyi sıfırla
            if (logoPreviewImg) {
                logoPreviewImg.style.display = 'none';
            }
            if (logoPlaceholder) {
                logoPlaceholder.style.display = 'flex';
            }

            teamModal.style.display = 'block';
        });
    }

    // Takım düzenleme modalını aç
    const editTeamBtns = document.querySelectorAll('.edit-team-btn');

    editTeamBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const teamId = this.getAttribute('data-team-id');

            // AJAX ile takım bilgilerini al
            fetch(`/Admin/GetTeamDetails?id=${teamId}`)
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        const team = data.team;

                        document.getElementById('modalTitle').textContent = 'Takım Düzenle';
                        document.getElementById('teamId').value = team.id;
                        document.getElementById('teamName').value = team.name;
                        document.getElementById('teamLeague').value = team.league;
                        document.getElementById('teamScore').value = team.score;

                        // Maç sonuçlarını doldur
                        const matches = team.matches;
                        for (let i = 0; i < matches.length && i < 5; i++) {
                            document.getElementById('match' + (i + 1)).value = matches[i];
                        }

                        // Logo önizlemeyi ayarla
                        if (team.imagePath) {
                            logoPreviewImg.src = team.imagePath;
                            logoPreviewImg.style.display = 'block';
                            logoPlaceholder.style.display = 'none';
                        } else {
                            logoPreviewImg.style.display = 'none';
                            logoPlaceholder.style.display = 'flex';
                        }

                        teamModal.style.display = 'block';
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Takım bilgileri alınırken bir hata oluştu.');
                });
        });
    });

    // Modalları kapat
    closeButtons.forEach(button => {
        button.addEventListener('click', function () {
            teamModal.style.display = 'none';
            matchesModal.style.display = 'none';
            deleteModal.style.display = 'none';
        });
    });

    // Takım formunu gönder
    const teamForm = document.getElementById('teamForm');

    if (teamForm) {
        teamForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData(this);

            // Maç sonuçlarını ekle
            const matchResults = [];
            for (let i = 1; i <= 5; i++) {
                matchResults.push(document.getElementById('match' + i).value);
            }
            formData.append('matchResults', matchResults.join(','));

            // AJAX ile gönder
            fetch('/Admin/SaveTeam', {
                method: 'POST',
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Başarılı ise sayfayı yenile
                        window.location.reload();
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Takım kaydedilirken bir hata oluştu.');
                });
        });
    }

    // Maç sonuçları düzenleme modalını aç
    const editMatchesBtns = document.querySelectorAll('.edit-matches-btn');
    const matchesModal = document.getElementById('matchesModal');

    editMatchesBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const teamId = this.getAttribute('data-team-id');
            const teamRow = document.querySelector(`tr[data-team-id="${teamId}"]`);
            const matchResults = teamRow.querySelectorAll('.match-result');

            document.getElementById('matchesTeamId').value = teamId;

            // Mevcut maç sonuçlarını doldur
            for (let i = 0; i < matchResults.length && i < 5; i++) {
                document.getElementById('editMatch' + (i + 1)).value = matchResults[i].textContent;
            }

            matchesModal.style.display = 'block';
        });
    });

    // Maç sonuçları formunu gönder
    const matchesForm = document.getElementById('matchesForm');

    if (matchesForm) {
        matchesForm.addEventListener('submit', function (e) {
            e.preventDefault();

            const teamId = document.getElementById('matchesTeamId').value;
            const matchResults = [];

            for (let i = 1; i <= 5; i++) {
                matchResults.push(document.getElementById('editMatch' + i).value);
            }

            // AJAX ile gönder
            fetch('/Admin/UpdateMatchResults', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    teamId: teamId,
                    matchResults: matchResults.join(',')
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Başarılı ise sayfayı yenile
                        window.location.reload();
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Maç sonuçları güncellenirken bir hata oluştu.');
                });
        });
    }

    // Takım silme modalını aç
    const deleteTeamBtns = document.querySelectorAll('.delete-team-btn');
    const deleteModal = document.getElementById('deleteModal');

    deleteTeamBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.stopPropagation();
            const teamId = this.getAttribute('data-team-id');

            document.getElementById('deleteTeamId').value = teamId;
            deleteModal.style.display = 'block';
        });
    });

    // Takım silme işlemini onayla
    const confirmDeleteBtn = document.getElementById('confirmDeleteBtn');

    if (confirmDeleteBtn) {
        confirmDeleteBtn.addEventListener('click', function () {
            const teamId = document.getElementById('deleteTeamId').value;

            // AJAX ile sil
            fetch('/Admin/DeleteTeam', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    id: teamId
                })
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        // Başarılı ise sayfayı yenile
                        window.location.reload();
                    } else {
                        alert('Hata: ' + data.message);
                    }
                })
                .catch(error => {
                    console.error('Hata:', error);
                    alert('Takım silinirken bir hata oluştu.');
                });
        });
    }

    // Sayfa dışına tıklandığında modalları kapat
    window.addEventListener('click', function (e) {
        if (e.target === teamModal) {
            teamModal.style.display = 'none';
        }
        if (e.target === matchesModal) {
            matchesModal.style.display = 'none';
        }
        if (e.target === deleteModal) {
            deleteModal.style.display = 'none';
        }
    });
});
