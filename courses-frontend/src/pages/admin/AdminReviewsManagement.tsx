"use client"

import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import AdminSidebar from "../../components/AdminSidebar"
import { getAllReviews, deleteReview } from "../../services/reviewService"
import { adminService } from "../../services/adminService"
import { checkToxicity } from "../../services/toxicService"
import { MessageSquareMore, Trash2, Shield, AlertTriangle, User } from "lucide-react"
import "../../styles/Admin.css"

type Review = {
    id: string
    comment: string
    userName: string
    score?: number
}

export default function AdminReviewsManagement() {
    const [toxicReviews, setToxicReviews] = useState<Review[]>([])
    const [loading, setLoading] = useState<boolean>(true)
    const [processingReview, setProcessingReview] = useState<string | null>(null)
    const [showDeleteConfirm, setShowDeleteConfirm] = useState<string | null>(null)
    const navigate = useNavigate()

    useEffect(() => {
        const init = async () => {
            try {
                await adminService.fetchDashboard()
                await fetchAndFilter()
            } catch (error) {
                console.error("Admin authorization error:", error)
                navigate("/")
            }
        }

        init()
    }, [navigate])

    const fetchAndFilter = async () => {
        setLoading(true)
        try {
            const allReviews = await getAllReviews()
            const filtered: Review[] = []

            for (const review of allReviews) {
                const score = await checkToxicity(review.comment)
                if (score > 0.2) {
                    filtered.push({ ...review, score })
                }
            }

            const sorted = filtered.sort((a, b) => (b.score ?? 0) - (a.score ?? 0))
            setToxicReviews(sorted)
        } finally {
            setLoading(false)
        }
    }

    const handleDelete = async (id: string) => {
        setProcessingReview(id)
        try {
            await deleteReview(id)
            setToxicReviews((prev) => prev.filter((r) => r.id !== id))
            setShowDeleteConfirm(null)
        } catch (error) {
            console.error("Error deleting review:", error)
        } finally {
            setProcessingReview(null)
        }
    }

    const handleKeep = (id: string) => {
        setToxicReviews((prev) => prev.filter((r) => r.id !== id))
    }

    const getToxicityLevel = (score: number) => {
        if (score >= 0.8) return { level: "Very High", className: "very-high" }
        if (score >= 0.6) return { level: "High", className: "high" }
        if (score >= 0.4) return { level: "Medium", className: "medium" }
        return { level: "Low", className: "low" }
    }

    const highRiskReviews = toxicReviews.filter((r) => (r.score ?? 0) >= 0.6)
    const veryHighRiskReviews = toxicReviews.filter((r) => (r.score ?? 0) >= 0.8)

    if (loading) {
        return (
            <div className="admin-layout">
                <AdminSidebar />
                <div className="admin-content">
                    <div className="loading-container">
                        <div className="loading-spinner"></div>
                        <p>Analyzing reviews for toxicity...</p>
                    </div>
                </div>
            </div>
        )
    }

    return (
        <div className="admin-layout">
            <AdminSidebar />
            <div className="admin-content">
                <div className="admin-header">
                    <div className="header-content">
                        <div className="header-text">
                            <h1>Review Management</h1>
                            <p>Monitor and manage potentially toxic comments</p>
                        </div>
                    </div>
                </div>

                <div className="stats-section">
                    {/* Stats */}
                    <div className="stats-grid">
                        <div className="stat-card">
                            <div className="stat-card-header">
                                <span className="stat-title">Toxic Reviews</span>
                                <div className="stat-icon-wrapper">
                                    {" "}
                                    {/* Added wrapper */}
                                    <MessageSquareMore size={20} className="stat-icon" />
                                </div>
                            </div>
                            <div className="stat-value danger">{toxicReviews.length}</div>
                            <div className="stat-subtitle">Requiring attention</div>
                        </div>

                        <div className="stat-card">
                            <div className="stat-card-header">
                                <span className="stat-title">High Risk</span>
                                <div className="stat-icon-wrapper">
                                    {" "}
                                    {/* Added wrapper */}
                                    <AlertTriangle size={20} className="stat-icon danger" />
                                </div>
                            </div>
                            <div className="stat-value danger">{highRiskReviews.length}</div>
                            <div className="stat-subtitle">Score ≥ 60%</div>
                        </div>

                        <div className="stat-card">
                            <div className="stat-card-header">
                                <span className="stat-title">Very High Risk</span>
                                <div className="stat-icon-wrapper">
                                    {" "}
                                    {/* Added wrapper */}
                                    <Shield size={20} className="stat-icon danger" />
                                </div>
                            </div>
                            <div className="stat-value danger">{veryHighRiskReviews.length}</div>
                            <div className="stat-subtitle">Score ≥ 80%</div>
                        </div>
                    </div>
                </div>

                <div className="main-section">
                    {/* Reviews List */}
                    <div className="content-card">
                        <div className="card-header">
                            <div className="card-title">
                                <MessageSquareMore size={20} />
                                Toxic Reviews
                            </div>
                            <div className="card-subtitle">Reviews flagged by AI toxicity detection (threshold: 20%)</div>
                        </div>
                        <div className="card-content">
                            {toxicReviews.length === 0 ? (
                                <div className="empty-state">
                                    <Shield size={48} className="empty-icon success" />
                                    <h3 className="empty-title success">All Clear!</h3>
                                    <p className="empty-subtitle">No toxic comments found.</p>
                                </div>
                            ) : (
                                <div className="scrollable-content tall">
                                    {toxicReviews.map((review) => {
                                        const toxicity = getToxicityLevel(review.score!)
                                        const isProcessing = processingReview === review.id

                                        return (
                                            <div key={review.id} className="review-item">
                                                {/* Header */}
                                                <div className="review-header">
                                                    <div className="review-user">
                                                        <User size={16} />
                                                        <div>
                                                            <p className="user-name">{review.userName}</p>
                                                            <p className="review-id">Review ID: {review.id}</p>
                                                        </div>
                                                    </div>
                                                    <span className={`risk-badge ${toxicity.className}`}>{toxicity.level} Risk</span>
                                                </div>

                                                {/* Comment */}
                                                <div className="review-comment">
                                                    <p>{review.comment}</p>
                                                </div>

                                                {/* Toxicity Score */}
                                                <div className="toxicity-score">
                                                    <div className="score-header">
                                                        <span>Toxicity Score</span>
                                                        <span className="score-value">{(review.score! * 100).toFixed(1)}%</span>
                                                    </div>
                                                    <div className="progress-bar">
                                                        <div
                                                            className={`progress-fill ${toxicity.className}`}
                                                            style={{ width: `${review.score! * 100}%` }}
                                                        ></div>
                                                    </div>
                                                </div>

                                                {/* Actions */}
                                                <div className="review-actions">
                                                    <button
                                                        className="btn btn-outline"
                                                        onClick={() => handleKeep(review.id)}
                                                        disabled={isProcessing}
                                                    >
                                                        <Shield size={16} />
                                                        Keep Review
                                                    </button>

                                                    <button
                                                        className="btn btn-danger"
                                                        disabled={isProcessing}
                                                        onClick={() => setShowDeleteConfirm(review.id)}
                                                    >
                                                        <Trash2 size={16} />
                                                        {isProcessing ? "Deleting..." : "Delete Review"}
                                                    </button>
                                                </div>
                                            </div>
                                        )
                                    })}
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Help Section */}
                    <div className="alert alert-info">
                        <AlertTriangle size={16} />
                        <div>
                            <strong>How toxicity detection works:</strong> Comments are analyzed using AI to detect potentially
                            harmful content. Scores above 20% are flagged for review. Higher scores indicate more likely toxic
                            content.
                        </div>
                    </div>
                </div>

                {/* Delete Confirmation Modal */}
                {showDeleteConfirm && (
                    <div className="modal-overlay" onClick={() => setShowDeleteConfirm(null)}>
                        <div className="modal" onClick={(e) => e.stopPropagation()}>
                            <div className="modal-header">
                                <div className="modal-title">
                                    <AlertTriangle size={20} className="danger" />
                                    Confirm Review Deletion
                                </div>
                            </div>
                            <div className="modal-content">
                                <p>Are you sure you want to delete this review? This action cannot be undone.</p>
                                <div className="warning-box">
                                    <AlertTriangle size={16} />
                                    <span>This action will permanently remove the review from the system.</span>
                                </div>
                            </div>
                            <div className="modal-actions">
                                <button className="btn btn-outline" onClick={() => setShowDeleteConfirm(null)}>
                                    Cancel
                                </button>
                                <button className="btn btn-danger" onClick={() => handleDelete(showDeleteConfirm)}>
                                    Delete
                                </button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    )
}
