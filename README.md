# 🧾 Project Documentation

### **Project Title:** HR Management system

### **Developer:** _Rajarshi Mondal_

---

## **1. Project Overview**

The **HR Leave Management System (HRLMS)** is a web-based platform designed to simplify and automate the process of managing employee leave requests within an organization.<br>
It provides distinct dashboards for **HR administrators** and **employees**, ensuring a smooth, transparent, and efficient leave management experience.

Each leave request is tracked through its entire lifecycle — from submission and review to approval or rejection — with all activities logged and visible to authorized users.

This system aims to:

- **Streamline** the leave application and approval workflow.
- **Enhance transparency** between HR and employees regarding leave status and availability.
- **Improve efficiency** in managing employee data, holidays, and notifications.
- **Provide real-time tracking** of pending requests, leave balances, and upcoming holidays.
- **Reduce manual effort** by automating notifications, approvals, and record updates.

---

## **2. Project Objectives**

- Simplify HR leave approval process.
- Centralize leave data management.
- Provide real-time tracking and notifications.
- Make the UI clean, responsive, and easy to use for both HR and employees.

---

## **3. Project Scope**

- The HR Leave Management System (HRLMS) covers the complete leave management lifecycle for both HR administrators and employees.
- It includes modules for employee registration, leave application, approval workflow, holiday management, and real-time tracking.
- The system ensures role-based access, allowing HR to manage employees and oversee organizational leave data, while employees can submit and monitor their requests.
- It also supports notifications, document uploads, and dashboard analytics for enhanced usability and transparency across the organization.

---

## **4. Technology Stack**

| Layer               | Technology              |
| ------------------- | ----------------------- |
| **Frontend**        | React + Vite + Axios    |
| **Backend**         | .NET 8 Web API + Dapper |
| **Database**        | PostgreSQL              |
| **Authentication**  | JWT Tokens              |
| **Version Control** | GitHub                  |
| **Testing Tools**   | Postman                 |

---

## 5. **User Roles and Core Features**

| Role                    |                        Description                        | Core Features                                                                                                                                                                                                              |
| :---------------------- | :-------------------------------------------------------: | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Employee**            |     Applies for leave and tracks its approval status.     | - Login with credentials<br>- Apply for leave (reason, date range, note, attachments)<br>- View leave history<br>- Track request status<br>- Check upcoming holidays                                                       |
| **HR (Administrator)**  | Manages employees, leave requests, and holiday schedules. | - Secure HR login<br>- View dashboard overview (availability, tracking, pending requests)<br>- Approve/Reject leave requests<br>- Add and manage employees<br>- Maintain holiday calendar<br>- Download Report and mail it |
| **System (Automation)** |    Handles background operations and data consistency.    | - Manage authentication via JWT<br>- Trigger email alerts<br>- Update leave statuses automatically<br>- Maintain database integrity and logs                                                                               |

---

## **6. Workflow**

### 1. Authentication

| Type          | Description                                                                                                                   |
| ------------- | ----------------------------------------------------------------------------------------------------------------------------- |
| Login/Sign-In | Common login portal for HR and Employees. HR login redirects to HR Dashboard; Employee login redirects to Employee Dashboard. |
| Sign-up       | Only HR can create new employee accounts via “Add Employee” module.                                                           |

### 2. HR Dashboard

| Feature                 | Description                                                                                |
| ----------------------- | ------------------------------------------------------------------------------------------ |
| Dashboard Overview      | View statistics like total employees, pending leave requests, and current availability.    |
| Pending Request Handler | Displays list of employee leave requests; HR can review, approve, or reject with comments. |
| Holiday List Management | Manage organization-wide holidays viewable by all employees.                               |
| Email Notification      | Sends email notification updates to employees about request status.                        |
| Add Employee            | HR adds new employees; credentials generated automatically at setup.                       |

### 3. Employee Dashboard

| Feature       | Description                                                                     |
| ------------- | ------------------------------------------------------------------------------- |
| Apply Leave   | Fill leave form (reason, description, dates, attachments). Sends request to HR. |
| Leave History | View past and current leave requests with approval statuses.                    |
| Next Holiday  | View company holidays at a glance or full calendar.                             |
| Track Request | Check progress of submitted leave requests in real time.                        |

---

## **7. Front end Designing**

#### HR Home Page
![](https://github.com/RajarshiCoding/HR-Leave-management/blob/ee065e2f4dd8599988dc71d85b892902c294e247/src/HR%20Front%20end%20design.png)

#### HR Leave Page
![](https://github.com/RajarshiCoding/HR-Leave-management/blob/ee065e2f4dd8599988dc71d85b892902c294e247/src/HR%20Frontend%20Leave%20page.png)

#### HR Leave Page
![](https://github.com/RajarshiCoding/HR-Leave-management/blob/ee065e2f4dd8599988dc71d85b892902c294e247/src/HR%20Frontend%20Calendar%20page.png)

#### Employee Dashboard
![](https://github.com/RajarshiCoding/HR-Leave-management/blob/ee065e2f4dd8599988dc71d85b892902c294e247/src/Employee%20frontend.png)

## **8. API calls**

### **_At a glance_ APIs**

| Method   | Endpoint                | Description                       | Request                           | Response                                                                                                                                                                                                          | Response Code |
| -------- | ----------------------- | --------------------------------- | --------------------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------- |
| **GET**  | `/api/dashboard`        | Get the data for at a glance page | ---                               | {name, dept, leave balance, leave taken, status}                                                                                                                                                                  | {200,500}     |
| **GET**  | `/api/dashboard/:empId` | Get the details for employee      | ---                               | {Employee ID, Name, Department, Designation / Role, Email / Contact, Joining Date, Leave Balance, Leaves Taken, Current Leave Status, Last Leave Date, Upcoming Leave Requests, Attendance / Presence (optional)} | {200,500}     |
| **GET**  | `/api/report/:empId`    | Get report for the employee       | ---                               | {file?, email redirection?}                                                                                                                                                                                       | {200,500}     |
| **POST** | `/api/add`              | Add a new employee to the DB      | {all data needed for an employee} | {emp id, email, passwd, mail redirection?}                                                                                                                                                                        | {201,500}     |

### **Leave APIs**

| Method   | Endpoint                | Description                                                      | Request                           | Response                            | Response Code |
| -------- | ----------------------- | ---------------------------------------------------------------- | --------------------------------- | ----------------------------------- | ------------- |
| **GET**  | `/api/leave`            | Get the data for pending page                                    | ---                               | {Emp name, start date, no. of days} | {200,500}     |
| **GET**  | `/api/leave/:requestId` | Get the details for the request                                  | ---                               | {request DB schema}                 | {200,500}     |
| **GET**  | `/api/leave/:empId`     | Get the details for the request of specific employee             | ---                               | {request DB schema}                 | {200,500}     |
| **POST** | `/api/leave/add`        | Add a new leave request to the DB                                | {all data needed for an employee} | {message}                           | {201,500}     |
| **PUT**  | `/api/leave/:requestId` | Accept or reject request                                         | {response:bool, note?:str}        | {mail redirection?}                 | {201,500}     |
| **GET**  | `/api/leave/isAny`      | checks if there is any pending requests left for the HR to check | ---                               | {response: Bool}                    | {200,500}     |

### **Calender APIs**

| Method     | Endpoint           | Description                 | Request                    | Response                          | Response Code |
| ---------- | ------------------ | --------------------------- | -------------------------- | --------------------------------- | ------------- |
| **GET**    | `/api/holiday`     | Get all the listed holidays | ---                        | {holiday id, title, description } | {200,500}     |
| **POST**   | `/api/holiday`     | Add new holiday             | {title, date, description} | {message}                         | {201,500}     |
| **PUT**    | `/api/holiday/:id` | Edit existing holiday       | {title, date, description} | {message}                         | {201,500}     |
| **DELETE** | `/api/holiday/:id` | Delete a holiday            | —                          | {message}                         | {201,500}     |

---

## **9. Database Structure**

### **1. Employees Table**

| Column Name       | Data Type    | Constraints          | Description                         |
| ----------------- | ------------ | -------------------- | ----------------------------------- |
| **emp_id**        | SERIAL       | PRIMARY KEY          | Unique Employee ID                  |
| **name**          | VARCHAR(100) | NOT NULL             | Employee full name                  |
| **email**         | VARCHAR(100) | UNIQUE, NOT NULL     | Employee email                      |
| **password_hash** | TEXT         | NOT NULL             | Hashed password                     |
| **password_salt** | TEXT         | NOT NULL             | Hash Salt                           |
| **department**    | VARCHAR(100) | NOT NULL             | Department name                     |
| **designation**   | VARCHAR(100) |                      | Employee designation / role         |
| **contact**       | VARCHAR(20)  |                      | Contact number                      |
| **joining_date**  | DATE         | DEFAULT CURRENT_DATE | Date of joining                     |
| **leave_balance** | INT          | DEFAULT 0            | Remaining leave days                |
| **leave_taken**   | INT          | DEFAULT 0            | Total leaves taken                  |
| **status**        | VARCHAR(20)  | DEFAULT 'Active'     | Active / Inactive / On Leave status |

---

### **2. LeaveRequests Table**

| Column Name     | Data Type   | Constraints                                            | Description                         |
| --------------- | ----------- | ------------------------------------------------------ | ----------------------------------- |
| **request_id**  | SERIAL      | PRIMARY KEY                                            | Unique leave request ID             |
| **emp_id**      | INT         | FOREIGN KEY → employees(emp_id)                        | Employee who raised the request     |
| **start_date**  | DATE        | NOT NULL                                               | Leave start date                    |
| **end_date**    | DATE        | NOT NULL                                               | Leave end date                      |
| **no_of_days**  | INT         | GENERATED ALWAYS AS (end_date - start_date + 1) STORED | Total days calculated automatically |
| **reason**      | TEXT        |                                                        | Reason for leave                    |
| **status**      | VARCHAR(20) | DEFAULT 'Pending'                                      | Pending / Approved / Rejected       |
| **hr_note**     | TEXT        |                                                        | HR comment or note                  |
| **applied_on**  | TIMESTAMP   | DEFAULT CURRENT_TIMESTAMP                              | Request creation time               |
| **reviewed_on** | TIMESTAMP   |                                                        | Date reviewed by HR                 |

### **3. Holidays Calendar Table**

| Column Name     | Data Type    | Constraints               | Description             |
| --------------- | ------------ | ------------------------- | ----------------------- |
| **holiday_id**  | SERIAL       | PRIMARY KEY               | Unique holiday ID       |
| **title**       | VARCHAR(100) | NOT NULL                  | Holiday name            |
| **description** | TEXT         |                           | Description or occasion |
| **date**        | DATE         | NOT NULL                  | Date of the holiday     |
| **created_at**  | TIMESTAMP    | DEFAULT CURRENT_TIMESTAMP | Record creation time    |

---

## **10. Timeline**

| Task                 | Duration | Start Date | End Date |
| -------------------- | -------- | ---------- | -------- |
| Requirement Analysis | 3 days   | 21-10-25   | 24-10-25 |
| Backend Development  | 5 days   | 25-10-25   | 30-10-25 |
| Design               | 2 days   | 31-10-25   | 2-11-25  |
| Frontend Development | 3 days   | 3-11-25    | 7-11-25  |

Deployment and submission: **_7-11-25_**

---
