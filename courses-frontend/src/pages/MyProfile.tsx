import { useEffect, useState } from "react";
import { config } from "../config";
import { fetchClient } from "../services/fetchClient";

type UserProfile = {
  firstName: string;
  lastName: string;
  email: string;
  profilePictureBase64?: string | null;
};

export default function MyProfile() {
  const [profile, setProfile] = useState<UserProfile>({
    firstName: "",
    lastName: "",
    email: "",
    profilePictureBase64: null,
  });
  const [isEditing, setIsEditing] = useState(false);
  const [previewImage, setPreviewImage] = useState<string | null>(null);

  useEffect(() => {
    fetchClient
      .fetch(config.apiEndpoints.getUserProfile, {
        credentials: "include",
        headers: { "Cache-Control": "no-cache" },
      })
      .then(res => {
        if (!res.ok) throw new Error("Unauthorized");
        return res.json();
      })
      .then(data => {
        const base64 = data.profilePictureBase64
          ? `data:image/png;base64,${data.profilePictureBase64}`
          : null;
        setProfile({
          firstName: data.firstName,
          lastName: data.lastName,
          email: data.email,
          profilePictureBase64: base64,
        });
        setPreviewImage(base64);
      })
      .catch(() => alert("Nie udało się pobrać danych profilu"));
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setProfile({ ...profile, [e.target.name]: e.target.value });
  };

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file && (file.type === "image/png" || file.type === "image/jpeg")) {
      const reader = new FileReader();
      reader.onloadend = () => {
        const base64 = reader.result?.toString() ?? null;
        setProfile(prev => ({ ...prev, profilePictureBase64: base64 }));
        setPreviewImage(base64);
      };
      reader.readAsDataURL(file);
    } else {
      alert("Wybierz plik PNG lub JPG");
    }
  };

  const handleSave = () => {
    const payload = {
      ...profile,
      profilePictureBase64: profile.profilePictureBase64
        ? profile.profilePictureBase64.split(",")[1]
        : null,
    };

    fetchClient
      .fetch(config.apiEndpoints.getUserProfile, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          "Cache-Control": "no-cache",
        },
        credentials: "include",
        body: JSON.stringify(payload),
      })
      .then(res => {
        if (!res.ok) throw new Error("Update failed");
        alert("Zapisano zmiany");
        setIsEditing(false);
      })
      .catch(() => alert("Nie udało się zapisać danych"));
  };

  return (
    <div className="max-w-xl mx-auto mt-10 text-white">
      <h1 className="text-2xl mb-6">My Profile</h1>

      {/* Zdjęcie profilowe */}
        <div className="mb-6 flex flex-col items-center">
    {previewImage ? (
      <img
        src={previewImage}
        alt="Profile"
        className="rounded border border-gray-500 mb-2 object-cover"
        style={{ width: "250px", height: "250px" }}
      />
    ) : (
      <div
        className="bg-gray-700 flex items-center justify-center text-gray-400 rounded border border-gray-500 mb-2"
        style={{ width: "250px", height: "250px" }}
      >
        No Image
      </div>
            )}
            {isEditing && (
              <input type="file" accept="image/png, image/jpeg" onChange={handleFileChange} />
            )}
      </div>

      <div className="mb-4">
        <span className="font-semibold">First Name:</span>{" "}
        {isEditing ? (
          <input
            className="bg-gray-800 border border-gray-600 p-1 rounded ml-2"
            name="firstName"
            value={profile.firstName}
            onChange={handleChange}
          />
        ) : (
          <span className="ml-2">{profile.firstName}</span>
        )}
      </div>

      <div className="mb-4">
        <span className="font-semibold">Last Name:</span>{" "}
        {isEditing ? (
          <input
            className="bg-gray-800 border border-gray-600 p-1 rounded ml-2"
            name="lastName"
            value={profile.lastName}
            onChange={handleChange}
          />
        ) : (
          <span className="ml-2">{profile.lastName}</span>
        )}
      </div>

      <div className="mb-4">
        <span className="font-semibold">Email:</span>{" "}
        {isEditing ? (
          <input
            className="bg-gray-800 border border-gray-600 p-1 rounded ml-2"
            name="email"
            value={profile.email}
            onChange={handleChange}
          />
        ) : (
          <span className="ml-2">{profile.email}</span>
        )}
      </div>

      {isEditing ? (
        <button
          onClick={handleSave}
          className="px-4 py-2 bg-green-600 rounded hover:bg-green-700 transition"
        >
          Zapisz zmiany
        </button>
      ) : (
        <button
          onClick={() => setIsEditing(true)}
          className="px-4 py-2 bg-blue-600 rounded hover:bg-blue-700 transition"
        >
          Edytuj dane
        </button>
      )}
    </div>
  );
}
