function createInitialBracket(teams) {
    // takımlar leaf olarak başlatılıyor
    let currentLevel = teams.map(team => ({
        team,
        left: null,
        right: null,
        winner: null
    }));

    while (currentLevel.length > 1) {
        const nextLevel = [];

        for (let i = 0; i < currentLevel.length; i += 2) {
            const left = currentLevel[i];
            const right = currentLevel[i + 1] || null; // Tek sayıda takım varsa bye alır

            nextLevel.push({
                team: null,        // henüz belirli değil, kazananlar yukarı çıkınca belirlenecek
                left: left,
                right: right,
                winner: null
            });
        }

        currentLevel = nextLevel;
    }

    return currentLevel[0]; // root node
}

function renderBracket(node) {
    const levels = [];

    function traverse(node, depth = 0) {
        if (!node) return;

        if (!levels[depth]) levels[depth] = [];
        levels[depth].push(node.team ? node.team.name : "[     ]");

        traverse(node.left, depth + 1);
        traverse(node.right, depth + 1);
    }

    traverse(node);

    const bracketContainer = document.getElementById("bracket");
    bracketContainer.innerHTML = ""; // Öncekini temizle

    levels.forEach(levelTeams => {
        const levelDiv = document.createElement("div");
        levelDiv.classList.add("bracket-row");

        levelTeams.forEach(name => {
            const teamDiv = document.createElement("div");
            teamDiv.classList.add("team-box");
            teamDiv.textContent = name;
            levelDiv.appendChild(teamDiv);
        });

        bracketContainer.appendChild(levelDiv);
    });
}


// API'den takımları al
fetch('/api/tournament/teams')
    .then(response => response.json())
    .then(teams => {
        const root = createInitialBracket(teams);
        
        renderBracket(root);
    })
    .catch(error => console.error('Hata:', error));
