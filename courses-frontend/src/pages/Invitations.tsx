import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { invitationService } from "../services/invitationService";
import Sidebar from "../components/Sidebar";
import { Check, X, Mail, Clock, Users, BookOpen } from "lucide-react";
import "../styles/Invitations.css";

interface Invitation {
	id: string;
	email: string;
	status: 0 | 1 | 2; // 0 = Pending, 1 = Accepted, 2 = Declined
	createdAt: string;
	courseName?: string;
	invitedBy?: string;
}

const Invitations = () => {
	const [invitations, setInvitations] = useState<Invitation[]>([]);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const { isLoggedIn } = useAuth();

	useEffect(() => {
		const fetchInvitations = async () => {
			try {
				setLoading(true);
				// Pobierz email użytkownika z localStorage lub z kontekstu
				const userEmail = localStorage.getItem("userEmail") || "";
				if (!userEmail) {
					setError("User email not found");
					return;
				}

				const data = await invitationService.getInvitationsByEmail(userEmail);
				console.log("Pobrane zaproszenia:", data); // Dodane logowanie
				console.log("Typ danych:", typeof data);
				console.log("Czy to array:", Array.isArray(data));
				if (Array.isArray(data) && data.length > 0) {
					console.log("Pierwsze zaproszenie:", data[0]);
					console.log("Status pierwszego zaproszenia:", data[0].status);
					console.log("Typ statusu:", typeof data[0].status);
					console.log("Czy status === 'Pending':", data[0].status === "Pending");
				}
				setInvitations(data);
			} catch (err) {
				console.error("Error fetching invitations:", err);
				setError("Failed to load invitations");
			} finally {
				setLoading(false);
			}
		};

		if (isLoggedIn) {
			fetchInvitations();
		}
	}, [isLoggedIn]);

	const handleAcceptInvitation = async (invitationId: string) => {
		try {
			console.log("Próba zaakceptowania zaproszenia:", invitationId); // Dodane logowanie
			await invitationService.acceptInvitation(invitationId);
			// Usuń zaproszenie z listy po zaakceptowaniu
			setInvitations(prev => prev.filter(inv => inv.id !== invitationId));
		} catch (err) {
			console.error("Error accepting invitation:", err);
			alert("Failed to accept invitation");
		}
	};

	const handleDeclineInvitation = async (invitationId: string) => {
		try {
			console.log("Próba odrzucenia zaproszenia:", invitationId); // Dodane logowanie
			await invitationService.declineInvitation(invitationId);
			// Usuń zaproszenie z listy po odrzuceniu
			setInvitations(prev => prev.filter(inv => inv.id !== invitationId));
		} catch (err) {
			console.error("Error declining invitation:", err);
			alert("Failed to decline invitation");
		}
	};

	const formatDate = (dateString: string) => {
		return new Date(dateString).toLocaleDateString("en-US", {
			year: "numeric",
			month: "short",
			day: "numeric",
			hour: "2-digit",
			minute: "2-digit"
		});
	};

	const getStatusColor = (status: number) => {
		switch (status) {
			case 1:
				return "#3dc55f";
			case 2:
				return "#dc3545";
			default:
				return "#ffc107";
		}
	};

	const getStatusText = (status: number) => {
		switch (status) {
			case 1:
				return "Accepted";
			case 2:
				return "Declined";
			default:
				return "Pending";
		}
	};

	if (loading) {
		return (
			<div className="invitations-layout">
				<Sidebar />
				<div className="invitations-container">
					<div className="invitations-loading">
						<Mail size={48} />
						<p>Loading invitations...</p>
					</div>
				</div>
			</div>
		);
	}

	if (error) {
		return (
			<div className="invitations-layout">
				<Sidebar />
				<div className="invitations-container">
					<div className="invitations-error">
						<X size={48} />
						<p>{error}</p>
					</div>
				</div>
			</div>
		);
	}

	return (
		<div className="invitations-layout">
			<Sidebar />
			<div className="invitations-container">
				<div className="invitations-header">
					<div className="header-content">
						<h1>Course Invitations</h1>
						<p>Manage your invitations to become a course creator</p>
					</div>
					<div className="header-stats">
						<div className="stat-item">
							<Mail size={20} />
							<span>{invitations.length} total</span>
						</div>
						<div className="stat-item">
							<Clock size={20} />
							<span>{invitations.filter(inv => inv.status === 0).length} pending</span>
						</div>
					</div>
				</div>

				{invitations.length === 0 ? (
					<div className="invitations-empty">
						<Mail size={64} />
						<h2>No invitations yet</h2>
						<p>You haven't received any course creator invitations yet.</p>
					</div>
				) : (
					<div className="invitations-grid">
						{invitations.map((invitation) => {
							console.log("Renderowanie zaproszenia:", invitation.id, "Status:", invitation.status); // Dodane logowanie
							return (
								<div key={invitation.id} className="invitation-card">
									<div className="invitation-header">
										<div className="course-info">
											<BookOpen size={20} />
											<h3>{invitation.courseName}</h3>
										</div>
										<div
											className="status-badge"
											style={{ backgroundColor: getStatusColor(invitation.status) }}
										>
											{getStatusText(invitation.status)}
										</div>
									</div>

									<div className="invitation-details">
										<div className="detail-item">
											<Users size={16} />
											<span>Invited by: {invitation.invitedBy || "Unknown"}</span>
										</div>
										<div className="detail-item">
											<Clock size={16} />
											<span>Sent on: {formatDate(invitation.createdAt)}</span>
										</div>
									</div>

									{invitation.status === 0 && (
										<div className="invitation-actions">
											<button
												className="btn-accept"
												onClick={() => handleAcceptInvitation(invitation.id)}
											>
												<Check size={16} />
												Accept
											</button>
											<button
												className="btn-decline"
												onClick={() => handleDeclineInvitation(invitation.id)}
											>
												<X size={16} />
												Decline
											</button>
										</div>
									)}
								</div>
							);
						})}
					</div>
				)}
			</div>
		</div>
	);
};

export default Invitations; 