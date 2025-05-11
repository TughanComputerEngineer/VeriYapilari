function createInitialBracket(teams) {
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
            const right = currentLevel[i + 1] || null;

            nextLevel.push({
                team: null,       
                left: left,
                right: right,
                winner: null
            });
        }

        currentLevel = nextLevel;
    }

    return currentLevel[0];
}

function truncateName(name, max = 12) {
    return name.length > max ? name.slice(0, max) + "…" : name;
}

function renderFullBracket(root) {
    const bracketContainer = document.getElementById("bracket");
    bracketContainer.innerHTML = "";

    let queue = [root];
    const levels = [];

    while (queue.length > 0) {
        const levelSize = queue.length;
        const currentLevel = [];
        let allNull = true;

        for (let i = 0; i < levelSize; i++) {
            const node = queue.shift();
            if (node) {
                currentLevel.push(node.team ? node.team.name : "[     ]");
                queue.push(node.left);
                queue.push(node.right);
                if (node.left || node.right) allNull = false;
            } else {
                currentLevel.push("[     ]");
                queue.push(null);
                queue.push(null);
            }
        }

        levels.push(currentLevel);

        if (allNull) break;
    }

    for (let i = 0; i < levels.length; i++) {
        const levelDiv = document.createElement("div");
        levelDiv.classList.add("bracket-row");

        levels[i].forEach(name => {
            const teamDiv = document.createElement("div");
            teamDiv.classList.add("team-box");
            teamDiv.textContent = truncateName(name);
            levelDiv.appendChild(teamDiv);
        });

        bracketContainer.appendChild(levelDiv);
    }

}
class TeamNode {
    constructor(id, name, league, score) {
        this.teamId = id;
        this.teamName = name;
        this.currentLeague = league;
        this.teamScore = score;
        this.next = null;
    }
}
class TeamQueue {
    constructor() {
        this.front = null; // Kuyruğun başı
        this.rear = null;  // Kuyruğun sonu
    }

    // Kuyruğa eleman ekleme
    enqueue(id, name, league, score) {
        const newNode = new TeamNode(id, name, league, score);

        if (this.rear === null) {
            // Kuyruk boşsa yeni node hem baş hem son olur
            this.front = this.rear = newNode;
        } else {
            this.rear.next = newNode;
            this.rear = newNode;
        }
    }

    // Kuyruktan eleman çıkarma
    dequeue() {
        if (this.front === null) {
            console.log("Kuyruk boş, çıkarılacak bir eleman yok.");
            return null;
        }

        const removedNode = this.front;
        this.front = this.front.next;

        if (this.front === null) {
            // Kuyruk boşaldıysa rear'ı da null yap
            this.rear = null;
        }

        return removedNode;
    }

    // Kuyruğu yazdırmak için yardımcı metod
    printQueue() {
        let current = this.front;
        while (current !== null) {
            console.log(`[${current.teamId}] ${current.teamName} (${current.currentLeague}) - ${current.teamScore}`);
            current = current.next;
        }
    }
}

// API'den takımları al
fetch('/api/tournament/teams')
    .then(response => response.json())
    .then(teams => {
        const root = createInitialBracket(teams);

        renderFullBracket(root);
    })
    .catch(error => console.error('Hata:', error));
