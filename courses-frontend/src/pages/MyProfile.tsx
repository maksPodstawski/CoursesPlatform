import { useEffect, useState } from "react";
import { config } from "../config";
import { fetchClient } from "../services/fetchClient";


export default function MyProfile() {
  const [profile, setProfile] = useState({
    firstName: "",
    lastName: "",
    email: "",
  });

  useEffect(() => {
     fetchClient
      .fetch(config.apiEndpoints.getUserProfile, {
  credentials: "include",
  headers: {
    "Cache-Control": "no-cache"
  }
    })
      .then(res => {
        if (!res.ok) throw new Error("Unauthorized");
        return res.json();
      })
      .then(data => setProfile({
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
      }))
      .catch(() => alert("Nie udało się pobrać danych profilu"));
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setProfile({ ...profile, [e.target.name]: e.target.value });
  };

  const handleSave = () => {
     fetchClient
      .fetch(config.apiEndpoints.getUserProfile,
       {
      method: "PUT",
      headers: { "Content-Type": "application/json", "Cache-Control": "no-cache" },
      credentials: "include",
      body: JSON.stringify(profile),
    })
      .then(res => {
        if (!res.ok) throw new Error("Update failed");
        alert("Zapisano zmiany");
      })
      .catch(() => alert("Nie udało się zapisać danych"));
  };

  return (
    <div className="max-w-xl mx-auto mt-10 text-white">
      <h1 className="text-2xl mb-4">My Profile</h1>

      <label className="block mb-2">First Name:</label>
      <input
        className="w-full p-2 mb-4 rounded bg-gray-800 border border-gray-600"
        name="firstName"
        value={profile.firstName}
        onChange={handleChange}
      />

      <label className="block mb-2">Last Name:</label>
      <input
        className="w-full p-2 mb-4 rounded bg-gray-800 border border-gray-600"
        name="lastName"
        value={profile.lastName}
        onChange={handleChange}
      />

      <label className="block mb-2">Email:</label>
      <input
        className="w-full p-2 mb-4 rounded bg-gray-800 border border-gray-600"
        name="email"
        value={profile.email}
        onChange={handleChange}
      />

      <button onClick={handleSave} className="px-4 py-2 bg-blue-600 rounded">
        Save
      </button>
    </div>
  );
}
