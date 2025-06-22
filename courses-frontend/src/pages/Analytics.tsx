import { BookOpen, DollarSign, Download, Star, Target, TrendingUp, Users } from "lucide-react";
import { useEffect, useState } from "react";
import {
	Bar,
	BarChart,
	CartesianGrid,
	Cell,
	Legend,
	Line,
	LineChart,
	Pie,
	PieChart,
	ResponsiveContainer,
	Tooltip,
	XAxis,
	YAxis,
} from "recharts";
import Sidebar from "../components/Sidebar";
import { type CreatorAnalytics, analyticsService } from "../services/analyticsService";
import "../styles/Analytics.css";

const Analytics = () => {
	const [analytics, setAnalytics] = useState<CreatorAnalytics | null>(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);
	const [selectedYear, setSelectedYear] = useState(new Date().getFullYear());

	useEffect(() => {
		const fetchAnalytics = async () => {
			try {
				setLoading(true);
				const data = await analyticsService.getMyAnalytics(selectedYear);
				setAnalytics(data);
			} catch (err) {
				setError("Failed to load analytics data");
				console.error("Analytics error:", err);
			} finally {
				setLoading(false);
			}
		};

		fetchAnalytics();
	}, [selectedYear]);

	const COLORS = ["#3dc55f", "#49e670", "#36b356", "#2f9a4a", "#288240"];

	const formatCurrency = (amount: number) => {
		return new Intl.NumberFormat("en-US", {
			style: "currency",
			currency: "USD",
		}).format(amount);
	};

	const formatPercentage = (value: number) => {
		return `${value.toFixed(1)}%`;
	};

	const getMonthName = (month: number) => {
		const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
		return months[month - 1];
	};

	const getYearOptions = () => {
		if (!analytics?.courses?.length) {
			return [<option key={selectedYear}>{selectedYear}</option>];
		}
		const firstCourseYear = new Date(analytics.courses[0].createdAt).getFullYear();
		const currentYear = new Date().getFullYear();
		const years = [];
		for (let year = currentYear; year >= firstCourseYear; year--) {
			years.push(
				<option key={year} value={year}>
					{year}
				</option>,
			);
		}
		return years;
	};

	const handleExport = () => {
		if (!analytics) return;

		const { totalCourses, totalStudents, totalRevenue, averageRating, totalReviews, completedStages, courses } =
			analytics;

		const escapeCsvField = (field: any): string => {
			if (field === null || field === undefined) {
				return "";
			}
			const stringField = String(field);
			if (stringField.includes(",") || stringField.includes('"') || stringField.includes("\n")) {
				return `"${stringField.replace(/"/g, '""')}"`;
			}
			return stringField;
		};

		const csvRows = [];

		csvRows.push(["Metric", "Value"]);
		csvRows.push(["Total Courses", totalCourses]);
		csvRows.push(["Total Students", totalStudents]);
		csvRows.push(["Total Revenue (USD)", totalRevenue]);
		csvRows.push(["Average Rating", averageRating.toFixed(1)]);
		csvRows.push(["Total Reviews", totalReviews]);
		csvRows.push(["Overall Course Completion Rate (%)", completedStages.toFixed(2)]);
		csvRows.push([]);

		csvRows.push(["Course Details"]);
		const headers = ["Course Name", "Students", "Revenue (USD)", "Rating", "Reviews", "Stages", "Completion Rate (%)"];
		csvRows.push(headers);

		courses.forEach((course) => {
			const row = [
				course.courseName,
				course.studentsCount,
				course.revenue,
				course.averageRating.toFixed(1),
				course.reviewsCount,
				course.stagesCount,
				course.completedStagesCount.toFixed(2),
			];
			csvRows.push(row);
		});

		const csvContent = csvRows.map((row) => row.map(escapeCsvField).join(",")).join("\r\n");

		const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
		const link = document.createElement("a");
		const url = URL.createObjectURL(blob);
		link.setAttribute("href", url);
		const date = new Date().toISOString().split("T")[0];
		link.setAttribute("download", `analytics-report-${date}.csv`);
		link.style.visibility = "hidden";
		document.body.appendChild(link);
		link.click();
		document.body.removeChild(link);
	};

	if (loading) {
		return (
			<div className="analytics-layout">
				<Sidebar />
				<div className="analytics-container loading-container">
					<div className="loading-spinner" />
					<p>Loading Analytics...</p>
				</div>
			</div>
		);
	}

	if (error) {
		return (
			<div className="analytics-layout">
				<Sidebar />
				<div className="analytics-container">
					<div className="error-message">{error}</div>
				</div>
			</div>
		);
	}

	if (!analytics) {
		return (
			<div className="analytics-layout">
				<Sidebar />
				<div className="analytics-container">
					<div className="no-data">No analytics data available</div>
				</div>
			</div>
		);
	}

	if (analytics.creatorId === "" || analytics.creatorName === "Not a creator yet") {
		return (
			<div className="analytics-layout">
				<Sidebar />
				<div className="analytics-container">
					<div className="analytics-header">
						<h1>Analytics Dashboard</h1>
						<p>Welcome to your analytics dashboard!</p>
					</div>
					<div className="no-creator-message">
						<h2>Start Your Creator Journey</h2>
						<p>
							You haven't created any courses yet. Once you create your first course, you'll be able to see detailed
							analytics here.
						</p>
						<div className="creator-actions">
							<a href="/add-course" className="create-course-btn">
								Create Your First Course
							</a>
						</div>
					</div>
				</div>
			</div>
		);
	}

	const monthlyRevenueData = analytics.monthlyRevenue.map((item) => ({
		month: getMonthName(item.month),
		revenue: item.revenue,
		sales: item.salesCount,
	}));

	const coursePerformanceData = analytics.topPerformingCourses.map((course) => ({
		name: course.courseName,
		students: course.studentsCount,
		revenue: course.revenue,
		rating: course.averageRating,
		completion: course.completionRate,
	}));

	const stageCompletionData = analytics.courses.flatMap((course) =>
		course.stages.map((stage) => ({
			name: `${course.courseName} - ${stage.stageName}`,
			completionRate: stage.completionRate,
			studentsStarted: stage.studentsStarted,
			studentsCompleted: stage.studentsCompleted,
		})),
	);

	return (
		<div className="analytics-layout">
			<Sidebar />
			<div className="analytics-container">
				<div className="analytics-header">
					<div className="header-content">
						<h1>Analytics Dashboard</h1>
						<p>Welcome back, {analytics.creatorName}</p>
					</div>
					<div className="header-controls">
						<div className="year-selector">
							<label htmlFor="year-select">Year:</label>
							<select id="year-select" value={selectedYear} onChange={(e) => setSelectedYear(Number(e.target.value))}>
								{getYearOptions()}
							</select>
						</div>
					</div>
				</div>

				{/* Key Metrics */}
				<div className="metrics-grid">
					<div className="metric-card">
						<div className="metric-icon">
							<BookOpen size={24} />
						</div>
						<div className="metric-content">
							<h3>Total Courses</h3>
							<p className="metric-value">{analytics.totalCourses}</p>
						</div>
					</div>

					<div className="metric-card">
						<div className="metric-icon">
							<Users size={24} />
						</div>
						<div className="metric-content">
							<h3>Total Students</h3>
							<p className="metric-value">{analytics.totalStudents}</p>
						</div>
					</div>

					<div className="metric-card">
						<div className="metric-icon">
							<DollarSign size={24} />
						</div>
						<div className="metric-content">
							<h3>Total Revenue</h3>
							<p className="metric-value">{formatCurrency(analytics.totalRevenue)}</p>
						</div>
					</div>

					<div className="metric-card">
						<div className="metric-icon">
							<Star size={24} />
						</div>
						<div className="metric-content">
							<h3>Average Rating</h3>
							<p className="metric-value">{analytics.averageRating.toFixed(1)}/5</p>
						</div>
					</div>

					<div className="metric-card">
						<div className="metric-icon">
							<Target size={24} />
						</div>
						<div className="metric-content">
							<h3>Total Reviews</h3>
							<p className="metric-value">{analytics.totalReviews}</p>
						</div>
					</div>

					<div className="metric-card">
						<div className="metric-icon">
							<TrendingUp size={24} />
						</div>
						<div className="metric-content">
							<h3>Course Completion Rate</h3>
							<p className="metric-value">{formatPercentage(analytics.completedStages)}</p>
						</div>
					</div>
				</div>

				<div className="charts-section">
					<div className="chart-container">
						<h2>Monthly Revenue ({selectedYear})</h2>
						<ResponsiveContainer width="100%" height={300}>
							<LineChart data={monthlyRevenueData}>
								<CartesianGrid strokeDasharray="3 3" stroke="#23232b" />
								<XAxis dataKey="month" stroke="#bbbbbb" />
								<YAxis stroke="#bbbbbb" />
								<Tooltip
									formatter={(value: any) => formatCurrency(Number(value))}
									contentStyle={{
										backgroundColor: "#15151a",
										border: "1px solid #23232b",
										borderRadius: "8px",
										color: "#fff",
									}}
								/>
								<Legend />
								<Line type="monotone" dataKey="revenue" stroke="#3dc55f" strokeWidth={2} />
							</LineChart>
						</ResponsiveContainer>
					</div>

					<div className="chart-container">
						<h2>Top Performing Courses</h2>
						<ResponsiveContainer width="100%" height={300}>
							<BarChart data={coursePerformanceData}>
								<CartesianGrid strokeDasharray="3 3" stroke="#23232b" />
								<XAxis dataKey="name" angle={-45} textAnchor="end" height={100} stroke="#bbbbbb" />
								<YAxis stroke="#bbbbbb" />
								<Tooltip
									formatter={(value: any, name: any, props: any) => {
										const courseData = props.payload;
										if (name === "Completion Rate") {
											return [
												`${Number(value).toFixed(2)}% (${formatCurrency(Number(courseData.revenue))} revenue)`,
												"Completion Rate",
											];
										}
										return [value, name];
									}}
									contentStyle={{
										backgroundColor: "#1e1e1e",
										border: "1px solid #2a2a2a",
										borderRadius: "8px",
										color: "#e0e0e0",
									}}
								/>
								<Legend />
								<Bar dataKey="completion" fill="#3dc55f" name="Completion Rate" />
							</BarChart>
						</ResponsiveContainer>
					</div>

					<div className="chart-container">
						<h2>Individual Stage Completion Rates</h2>
						<ResponsiveContainer width="100%" height={300}>
							<BarChart data={stageCompletionData.slice(0, 10)}>
								<CartesianGrid strokeDasharray="3 3" stroke="#23232b" />
								<XAxis dataKey="name" angle={-45} textAnchor="end" height={120} stroke="#bbbbbb" />
								<YAxis stroke="#bbbbbb" />
								<Tooltip
									formatter={(value: any, name: any, props: any) => {
										const stageData = props.payload;
										return [
											`${Number(value).toFixed(2)}% (${stageData.studentsStarted} students started)`,
											"Completion Rate",
										];
									}}
									contentStyle={{
										backgroundColor: "#1e1e1e",
										border: "1px solid #2a2a2a",
										borderRadius: "8px",
										color: "#e0e0e0",
									}}
								/>
								<Legend />
								<Bar dataKey="completionRate" fill="#3dc55f" name="Completion Rate" />
							</BarChart>
						</ResponsiveContainer>
					</div>

					<div className="chart-container">
						<h2>Revenue Distribution by Course</h2>
						<ResponsiveContainer width="100%" height={300}>
							<PieChart>
								<Pie
									data={analytics.courses.map((course) => ({
										name: course.courseName,
										value: course.revenue,
									}))}
									cx="50%"
									cy="50%"
									labelLine={false}
									label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
									outerRadius={80}
									fill="#3dc55f"
									dataKey="value"
								>
									{analytics.courses.map((entry, index) => (
										<Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
									))}
								</Pie>
								<Tooltip
									formatter={(value: any) => formatCurrency(Number(value))}
									contentStyle={{
										backgroundColor: "#15151a",
										border: "1px solid #23232b",
										borderRadius: "8px",
										color: "#fff",
									}}
								/>
							</PieChart>
						</ResponsiveContainer>
					</div>
				</div>

				<div className="course-details-section">
					<h2>Course Details</h2>
					<div className="course-table">
						<table>
							<thead>
								<tr>
									<th>Course Name</th>
									<th>Students</th>
									<th>Revenue</th>
									<th>Rating</th>
									<th>Reviews</th>
									<th>Stages</th>
									<th>Completion Rate</th>
								</tr>
							</thead>
							<tbody>
								{analytics.courses.map((course) => (
									<tr key={course.courseId}>
										<td>{course.courseName}</td>
										<td>{course.studentsCount}</td>
										<td>{formatCurrency(course.revenue)}</td>
										<td>{course.averageRating.toFixed(1)}/5</td>
										<td>{course.reviewsCount}</td>
										<td>{course.stagesCount}</td>
										<td>{formatPercentage(course.completedStagesCount)}</td>
									</tr>
								))}
							</tbody>
						</table>
					</div>
					<button type="button" className="export-btn" onClick={handleExport}>
						<Download size={20} />
						Export Report
					</button>
				</div>
			</div>
		</div>
	);
};

export default Analytics;
