import React, { useState } from "react";

export default function LoginPage() {
    const [userName, setUserName] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = (e) => {
        e.preventDefault();

        if (!userName || !password) {
            setError("Please enter both account and password.");
        } else {
            setError("");
            // Handle login logic here
            alert("Logged in!");
        }
    };

    return (
        <div style={{ maxWidth: 400, margin: "100px auto", padding: 24, border: "1px solid #ccc", borderRadius: 8 }}>
            <h2>Login</h2>
            <form onSubmit={handleSubmit}>
                <div style={{ marginBottom: 16 }}>
                    <label>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={e => setEmail(e.target.value)}
                        style={{ width: "100%", padding: 8, marginTop: 4 }}
                        required
                    />
                </div>
                <div style={{ marginBottom: 16 }}>
                    <label>Password</label>
                    <input
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        style={{ width: "100%", padding: 8, marginTop: 4 }}
                        required
                    />
                </div>
                {error && <div style={{ color: "red", marginBottom: 16 }}>{error}</div>}
                <button type="submit" style={{ width: "100%", padding: 10 }}>Login</button>
            </form>
        </div>
    );
}