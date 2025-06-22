import type React from "react";
import { useEffect, useState } from "react";
import { config } from "../config";
import { fetchClient } from "../services/fetchClient";
import type { UserProfile } from "../types/user";

export default function MyProfile() {
	const [profile, setProfile] = useState<UserProfile>({
		firstName: "",
		lastName: "",
		email: "",
		userName: "",
		phoneNumber: "",
		profilePictureBase64: null,
	});
	const [editedProfile, setEditedProfile] = useState<UserProfile>(profile);
	const [previewImage, setPreviewImage] = useState<string | null>(null);
	const [isEditing, setIsEditing] = useState(false);

	useEffect(() => {
		fetchClient
			.fetch(config.apiEndpoints.getUserProfile, {
				credentials: "include",
				headers: { "Cache-Control": "no-cache" },
			})
			.then((res) => {
				if (!res.ok) throw new Error("Unauthorized");
				return res.json();
			})
			.then((data) => {
				const base64 = data.profilePictureBase64 ? `data:image/png;base64,${data.profilePictureBase64}` : null;

				const fetchedProfile: UserProfile = {
					firstName: data.firstName || "",
					lastName: data.lastName || "",
					email: data.email || "",
					userName: data.userName || "",
					phoneNumber: data.phoneNumber || "",
					profilePictureBase64: base64,
				};

				setProfile(fetchedProfile);
				setEditedProfile(fetchedProfile);
				setPreviewImage(base64);
			})
			.catch(() => alert("Failed to load profile data"));
	}, []);

	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		setEditedProfile({ ...editedProfile, [e.target.name]: e.target.value });
	};

	const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const file = e.target.files?.[0];
		if (file && (file.type === "image/png" || file.type === "image/jpeg")) {
			const reader = new FileReader();
			reader.onloadend = () => {
				const base64 = reader.result?.toString() ?? null;
				setEditedProfile((prev) => ({
					...prev,
					profilePictureBase64: base64,
				}));
				setPreviewImage(base64);
			};
			reader.readAsDataURL(file);
		} else {
			alert("Only PNG or JPG files are allowed.");
		}
	};

	const handleSave = () => {
		if (editedProfile.phoneNumber && !/^\d{0,20}$/.test(editedProfile.phoneNumber)) {
			alert("Phone number can contain only digits (up to 20).");
			return;
		}

		const payload = {
			...editedProfile,
			profilePictureBase64: editedProfile.profilePictureBase64
				? editedProfile.profilePictureBase64.split(",")[1]
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
			.then((res) => {
				if (!res.ok) throw new Error("Update failed");
				alert("Changes saved");
				setProfile(editedProfile);
				setIsEditing(false);
			})
			.catch(() => alert("Failed to save changes"));
	};

	const handleCancel = () => {
		setEditedProfile(profile);
		setPreviewImage(profile.profilePictureBase64);
		setIsEditing(false);
	};

	return (
		<div className="profile-container">
			<h2>My Profile</h2>

			<div style={{ marginBottom: "1rem" }}>
				{previewImage ? (
					<img
						src={previewImage}
						alt="Profile"
						style={{ width: "150px", height: "150px", borderRadius: "50%", objectFit: "cover", marginBottom: "0.5rem" }}
					/>
				) : (
					<div
						style={{
							width: "150px",
							height: "150px",
							borderRadius: "50%",
							backgroundColor: "#ccc",
							display: "flex",
							justifyContent: "center",
							alignItems: "center",
							marginBottom: "0.5rem",
						}}
					>
						No Image
					</div>
				)}

				{isEditing && <input type="file" accept="image/png, image/jpeg" onChange={handleFileChange} />}
			</div>

			{!isEditing ? (
				<>
					<div className="profile-field">
						<label>First Name:</label>
						<span>{profile.firstName}</span>
					</div>
					<div className="profile-field">
						<label>Last Name:</label>
						<span>{profile.lastName}</span>
					</div>
					<div className="profile-field">
						<label>Email:</label>
						<span>{profile.email}</span>
					</div>
					<div className="profile-field">
						<label>Username:</label>
						<span>{profile.userName}</span>
					</div>
					<div className="profile-field">
						<label>Phone Number:</label>
						<span>{profile.phoneNumber}</span>
					</div>

					<div className="profile-actions">
						<button className="btn primary" onClick={() => setIsEditing(true)}>
							Edit
						</button>
					</div>
				</>
			) : (
				<>
					<div className="profile-field">
						<label>First Name:</label>
						<input type="text" name="firstName" value={editedProfile.firstName} onChange={handleChange} />
					</div>
					<div className="profile-field">
						<label>Last Name:</label>
						<input type="text" name="lastName" value={editedProfile.lastName} onChange={handleChange} />
					</div>
					<div className="profile-field">
						<label>Email:</label>
						<input type="email" name="email" value={editedProfile.email} onChange={handleChange} />
					</div>
					<div className="profile-field">
						<label>Username:</label>
						<input type="text" name="userName" value={editedProfile.userName} onChange={handleChange} />
					</div>
					<div className="profile-field">
						<label>Phone Number:</label>
						<input type="tel" name="phoneNumber" value={editedProfile.phoneNumber} onChange={handleChange} />
					</div>

					<div className="profile-actions">
						<button className="btn primary" onClick={handleSave}>
							Save
						</button>
						<button className="btn primary" onClick={handleCancel}>
							Cancel
						</button>
					</div>
				</>
			)}
		</div>
	);
}
