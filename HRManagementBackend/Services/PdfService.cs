using HRManagementBackend.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HRManagementBackend.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateEmployeeReport(Employee employee, IEnumerable<LeaveRequest> leaves)
        {
            try
            {
                // Filter leaves from the last 30 days
                var recentLeaves = leaves?
                    .Where(l => l.StartDate >= DateTime.Now.AddDays(-30))
                    .OrderByDescending(l => l.StartDate)
                    .ToList();

                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(12));

                        // Header
                        page.Header()
                            .Text($"Employee Report: {employee.Name}")
                            .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

                        // Content
                        page.Content().PaddingVertical(10).Column(column =>
                        {
                            column.Spacing(10);

                            // Employee Details Section
                            column.Item().Text("Employee Details").Bold().FontSize(14);
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.ConstantColumn(150);
                                    c.RelativeColumn();
                                });

                                void Row(string label, string value)
                                {
                                    table.Cell().Element(CellStyle).Text(label);
                                    table.Cell().Element(CellStyle).Text(value ?? "-");
                                }

                                Row("Employee ID:", employee.EmpId.ToString());
                                Row("Name:", employee.Name);
                                Row("Email:", employee.Email);
                                Row("Department:", employee.Department);
                                Row("Designation:", employee.Designation);
                                Row("Contact:", employee.Contact);
                                Row("Joining Date:", employee.JoiningDate.ToString("yyyy-MM-dd"));
                                Row("Date of Birth:", employee.DOB.ToString("yyyy-MM-dd"));
                                Row("Leave Balance:", employee.LeaveBalance.ToString());
                                Row("Leave Taken:", employee.LeaveTaken.ToString());

                                static IContainer CellStyle(IContainer container) =>
                                    container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                            });

                            // Leave Records Section
                            column.Item().PaddingTop(20).Text("Leave Records (Last 30 Days)").Bold().FontSize(14);

                            if (recentLeaves == null || !recentLeaves.Any())
                            {
                                column.Item().Text("No leave records found in the last 30 days.").Italic();
                            }
                            else
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(2); // Start Date
                                        c.RelativeColumn(2); // End Date
                                        c.RelativeColumn(1); // Days
                                        c.RelativeColumn(3); // Reason
                                        c.RelativeColumn(2); // Status
                                    });

                                    // Table Header
                                    table.Header(header =>
                                    {
                                        header.Cell().Element(HeaderCellStyle).Text("Start Date").Bold();
                                        header.Cell().Element(HeaderCellStyle).Text("End Date").Bold();
                                        header.Cell().Element(HeaderCellStyle).Text("Days").Bold();
                                        header.Cell().Element(HeaderCellStyle).Text("Reason").Bold();
                                        header.Cell().Element(HeaderCellStyle).Text("Status").Bold();

                                        static IContainer HeaderCellStyle(IContainer container) =>
                                            container.BorderBottom(2).BorderColor(Colors.Grey.Darken1).PaddingBottom(5);
                                    });

                                    // Data Rows
                                    foreach (var leave in recentLeaves)
                                    {
                                        table.Cell().Element(CellStyle).Text(leave.StartDate.ToString("yyyy-MM-dd"));
                                        table.Cell().Element(CellStyle).Text(leave.EndDate.ToString("yyyy-MM-dd"));
                                        table.Cell().Element(CellStyle).Text(leave.NoOfDays.ToString());
                                        table.Cell().Element(CellStyle).Text(leave.Reason ?? "-");
                                        table.Cell().Element(CellStyle).Text(leave.Status);

                                        static IContainer CellStyle(IContainer container) =>
                                            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);
                                    }
                                });
                            }
                        });

                        // Footer
                        page.Footer().AlignCenter().Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to generate employee report PDF.", ex);
            }
        }
    }
}
