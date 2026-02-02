console.log("voting.js geladen");

let isAdmin = false;
let password = localStorage.getItem("adminPass") || "admin";
let votingActive = localStorage.getItem("votingActive") !== "false";

let favoriten = new Set();
let topfavorit = null;

document.addEventListener("DOMContentLoaded", () => {
    console.log("DOM geladen – Initialisierung läuft");

    const info = document.getElementById("info");
    const message = document.getElementById("message");
    const submitBtn = document.getElementById("submitBtn");
    const adminPanel = document.getElementById("adminPanel");
    const adminControls = document.getElementById("adminControls");
    const summaryTable = document.querySelector("#summaryTable tbody");
    const adminStatus = document.getElementById("adminStatus");

    window.handleClick = function (id, btn) {
        if (!votingActive && !isAdmin) return;

        if (topfavorit === id) {
            topfavorit = null;
            btn.classList.remove("topfavorit");
        } else if (favoriten.has(id)) {
            favoriten.delete(id);
            btn.classList.remove("favorit");
        } else if (favoriten.size < 5) {
            favoriten.add(id);
            btn.classList.add("favorit");
        } else {
            message.textContent = "Maximal 5 Favoriten erlaubt.";
            message.style.color = "red";
        }

        updateInfo();
    };

    window.toggleTopFavorit = function (id, btn) {
        if (!votingActive && !isAdmin) return;

        const old = document.querySelector(".topfavorit");
        if (old) old.classList.remove("topfavorit");

        topfavorit = id;
        favoriten.delete(id);

        document.querySelectorAll(".button").forEach(b => {
            if (parseInt(b.dataset.id) === id) b.classList.remove("favorit");
        });

        btn.classList.add("topfavorit");
        message.textContent = "";

        updateInfo();
    };

    function updateInfo() {
        const favoritenCount = favoriten.size;
        const total = favoritenCount + (topfavorit ? 1 : 0);

        info.textContent = `Favoriten: ${favoritenCount}, Topfavorit: ${topfavorit ?? "–"}`;

        submitBtn.disabled = total !== 6;
    }

    window.submitVote = async function () {
        if (submitBtn.disabled) return;

        const results = JSON.parse(localStorage.getItem("results") || "[]");
        results.push({
            favoriten: Array.from(favoriten),
            topfavorit,
            time: new Date().toLocaleString()
        });
        localStorage.setItem("results", JSON.stringify(results));
        localStorage.setItem("hasVoted", "true");

        try {
            await fetch("/api/voting/submit", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({
                    favoriten: Array.from(favoriten),
                    topfavorit: topfavorit
                })
            });
        } catch (err) {
            console.error("Fehler beim Senden an den Server:", err);
        }

        disableVoting();
        message.style.color = "green";
        message.textContent = "Danke für Ihre Bewertung!";
        renderSummary();
    };

    function disableVoting() {
        document.querySelectorAll(".button").forEach(b => b.disabled = true);
        submitBtn.disabled = true;
    }

    function enableVoting() {
        document.querySelectorAll(".button").forEach(b => {
            b.disabled = false;
            b.classList.remove("favorit", "topfavorit");
        });
        favoriten.clear();
        topfavorit = null;
        updateInfo();
    }

    window.checkAdmin = function () {
        const code = document.getElementById("adminCode").value;
        if (code === password) {
            isAdmin = true;
            adminControls.style.display = "block";
            renderSummary();
        } else {
            alert("Falscher Admin-Code");
        }
    };

    window.logoutAdmin = function () {
        isAdmin = false;
        adminControls.style.display = "none";
    };

    window.startVoting = function () {
        votingActive = true;
        localStorage.setItem("votingActive", "true");
        adminStatus.textContent = "Abstimmung wurde gestartet.";
    };

    window.stopVoting = function () {
        votingActive = false;
        localStorage.setItem("votingActive", "false");
        adminStatus.textContent = "Abstimmung wurde gestoppt.";
    };

    window.clearLocalVote = function () {
        localStorage.removeItem("hasVoted");
        enableVoting();
        adminStatus.textContent = "Dieses Gerät darf erneut abstimmen.";
    };

    window.resetVotes = function () {
        if (confirm("Alle Bewertungen löschen?")) {
            localStorage.removeItem("results");
            renderSummary();
            adminStatus.textContent = "Alle Bewertungen wurden gelöscht.";
        }
    };

    window.changePassword = function () {
        const newPass = prompt("Neues Admin-Passwort eingeben:");
        if (newPass) {
            password = newPass;
            localStorage.setItem("adminPass", newPass);
            adminStatus.textContent = "Passwort wurde geändert.";
        }
    };

    window.renderSummary = function () {
        if (!summaryTable) return;

        const results = JSON.parse(localStorage.getItem("results") || "[]");
        const favoritenCount = Array(20).fill(0);
        const topfavoritenCount = Array(20).fill(0);

        results.forEach(entry => {
            entry.favoriten.forEach(i => favoritenCount[i - 1]++);
            if (entry.topfavorit) topfavoritenCount[entry.topfavorit - 1]++;
        });

        summaryTable.innerHTML = "";
        for (let i = 0; i < 20; i++) {
            const row = document.createElement("tr");
            const punkte = favoritenCount[i] + topfavoritenCount[i] * 2;
            row.innerHTML = `<td>${i + 1}</td><td>${favoritenCount[i]}</td><td>${topfavoritenCount[i]}</td><td>${punkte}</td>`;
            summaryTable.appendChild(row);
        }
    };

    if (localStorage.getItem("hasVoted") === "true") {
        disableVoting();
    }

    updateInfo();
    renderSummary();
});