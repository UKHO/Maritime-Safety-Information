{
  "lenses": {
    "0": {
      "order": 0,
      "parts": {
        "0": {
          "position": {
            "x": 0,
            "y": 0,
            "colSpan": 6,
            "rowSpan": 4
          },
          "metadata": {
            "inputs": [
              {
                "name": "options",
                "isOptional": true
              },
              {
                "name": "sharedTimeRange",
                "isOptional": true
              }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
              "content": {
                "options": {
                  "chart": {
                    "metrics": [
                      {
                        "resourceMetadata": {
                          "id": "/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Web/sites/${web_app_name}"
                        },
                        "name": "HttpResponseTime",
                        "aggregationType": 4,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                          "displayName": "Response Time",
                          "resourceDisplayName": "${web_app_name}"
                        }
                      }
                    ],
                    "title": "REQUEST LATENCY - (Avg Response Time for ${web_app_name})",
                    "titleKind": 2,
                    "visualization": {
                      "chartType": 2,
                      "legendVisualization": {
                        "hideSubtitle": false,
                        "isVisible": true,
                        "position": 2
                      },
                      "axisVisualization": {
                        "x": {
                          "axisType": 2,
                          "isVisible": true
                        },
                        "y": {
                          "axisType": 1,
                          "isVisible": true
                        }
                      },
                      "disablePinning": true
                    }
                  }
                }
              }
            }
          }
        },
        "1": {
          "position": {
            "x": 6,
            "y": 0,
            "colSpan": 6,
            "rowSpan": 4
          },
          "metadata": {
            "inputs": [
              {
                "name": "options",
                "isOptional": true
              },
              {
                "name": "sharedTimeRange",
                "isOptional": true
              }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
              "content": {
                "options": {
                  "chart": {
                    "metrics": [
                      {
                        "resourceMetadata": {
                          "id": "/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Web/sites/${web_app_name}"
                        },
                        "name": "BytesReceived",
                        "aggregationType": 1,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                          "displayName": "Data In",
                          "resourceDisplayName": "${web_app_name}"
                        }
                      }
                    ],
                    "title": "THROUGHPUT - (Sum Data In for ${web_app_name})",
                    "titleKind": 2,
                    "visualization": {
                      "chartType": 2,
                      "legendVisualization": {
                        "hideSubtitle": false,
                        "isVisible": true,
                        "position": 2
                      },
                      "axisVisualization": {
                        "x": {
                          "axisType": 2,
                          "isVisible": true
                        },
                        "y": {
                          "axisType": 1,
                          "isVisible": true
                        }
                      },
                      "disablePinning": true
                    }
                  }
                }
              }
            }
          }
        },
        "2": {
          "position": {
            "x": 12,
            "y": 0,
            "colSpan": 6,
            "rowSpan": 4
          },
          "metadata": {
            "inputs": [
              {
                "name": "options",
                "isOptional": true
              },
              {
                "name": "sharedTimeRange",
                "isOptional": true
              }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
              "content": {
                "options": {
                  "chart": {
                    "metrics": [
                      {
                        "resourceMetadata": {
                          "id": "/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Web/sites/${web_app_name}"
                        },
                        "name": "BytesSent",
                        "aggregationType": 1,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                          "displayName": "Data Out",
                          "resourceDisplayName": "${web_app_name}"
                        }
                      }
                    ],
                    "title": "THROUGHPUT - (Sum Data Out for ${web_app_name})",
                    "titleKind": 2,
                    "visualization": {
                      "chartType": 2,
                      "legendVisualization": {
                        "hideSubtitle": false,
                        "isVisible": true,
                        "position": 2
                      },
                      "axisVisualization": {
                        "x": {
                          "axisType": 2,
                          "isVisible": true
                        },
                        "y": {
                          "axisType": 1,
                          "isVisible": true
                        }
                      },
                      "disablePinning": true
                    }
                  }
                }
              }
            }
          }
        },
        "3": {
          "position": {
            "x": 0,
            "y": 4,
            "colSpan": 6,
            "rowSpan": 4
          },
          "metadata": {
            "inputs": [
              {
                "name": "options",
                "isOptional": true
              },
              {
                "name": "sharedTimeRange",
                "isOptional": true
              }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
              "content": {
                "options": {
                  "chart": {
                    "metrics": [
                      {
                        "resourceMetadata": {
                          "id": "/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Web/sites/${web_app_name}"
                        },
                        "name": "Requests",
                        "aggregationType": 1,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                          "displayName": "Requests",
                          "resourceDisplayName": "${web_app_name}"
                        }
                      }
                    ],
                    "title": "REQUESTS (Sum Requests for ${web_app_name})",
                    "titleKind": 2,
                    "visualization": {
                      "chartType": 2,
                      "legendVisualization": {
                        "isVisible": true,
                        "position": 2,
                        "hideSubtitle": false
                      },
                      "axisVisualization": {
                        "x": {
                          "isVisible": true,
                          "axisType": 2
                        },
                        "y": {
                          "isVisible": true,
                          "axisType": 1
                        }
                      },
                      "disablePinning": true
                    }
                  }
                }
              }
            }
          }
        },
        "4": {
          "position": {
            "x": 6,
            "y": 4,
            "colSpan": 6,
            "rowSpan": 4
          },
          "metadata": {
            "inputs": [
              {
                "name": "options",
                "isOptional": true
              },
              {
                "name": "sharedTimeRange",
                "isOptional": true
              }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
              "content": {
                "options": {
                  "chart": {
                    "metrics": [
                      {
                        "resourceMetadata": {
                          "id": "/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Web/sites/${web_app_name}"
                        },
                        "name": "HealthCheckStatus",
                        "aggregationType": 7,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                          "displayName": "Health check status",
                          "resourceDisplayName": "${web_app_name}"
                        }
                      }
                    ],
                    "title": "AVAILABILITY - (Count Health check status for ${web_app_name})",
                    "titleKind": 2,
                    "visualization": {
                      "chartType": 2,
                      "legendVisualization": {
                        "hideSubtitle": false,
                        "isVisible": true,
                        "position": 2
                      },
                      "axisVisualization": {
                        "x": {
                          "axisType": 2,
                          "isVisible": true
                        },
                        "y": {
                          "axisType": 1,
                          "isVisible": true
                        }
                      },
                      "disablePinning": true
                    }
                  }
                }
              }
            }
          }
        },
        "5": {
          "position": {
            "x": 12,
            "y": 4,
            "colSpan": 6,
            "rowSpan": 4
          },
          "metadata": {
            "inputs": [
              {
                "name": "options",
                "isOptional": true
              },
              {
                "name": "sharedTimeRange",
                "isOptional": true
              }
            ],
            "type": "Extension/HubsExtension/PartType/MonitorChartPart",
            "settings": {
              "content": {
                "options": {
                  "chart": {
                    "metrics": [
                      {
                        "resourceMetadata": {
                          "id": "/subscriptions/${subscription_id}/resourceGroups/${resource_group_name}/providers/Microsoft.Web/sites/${web_app_name}"
                        },
                        "name": "Http5xx",
                        "aggregationType": 7,
                        "namespace": "microsoft.web/sites",
                        "metricVisualization": {
                          "displayName": "Http Server Errors",
                          "resourceDisplayName": "${web_app_name}"
                        }
                      }
                    ],
                    "title": "ERROR RATE - (Count Http Server Errors for ${web_app_name})",
                    "titleKind": 2,
                    "visualization": {
                      "chartType": 2,
                      "legendVisualization": {
                        "hideSubtitle": false,
                        "isVisible": true,
                        "position": 2
                      },
                      "axisVisualization": {
                        "x": {
                          "axisType": 2,
                          "isVisible": true
                        },
                        "y": {
                          "axisType": 1,
                          "isVisible": true
                        }
                      },
                      "disablePinning": true
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
  },
  "metadata": {
    "model": {
      "timeRange": {
        "value": {
          "relative": {
            "duration": 24,
            "timeUnit": 1
          }
        },
        "type": "MsPortalFx.Composition.Configuration.ValueTypes.TimeRange"
      },
      "filterLocale": {
        "value": "en-us"
      },
      "filters": {
        "value": {
          "MsPortalFx_TimeRange": {
            "model": {
              "format": "local",
              "granularity": "auto",
              "relative": "7d"
            },
            "displayCache": {
              "name": "Local Time",
              "value": "Past 7 days"
            },
            "filteredPartIds": [
              "StartboardPart-MonitorChartPart-497080ff-3f3b-4555-9c34-2d55afa301a8",
              "StartboardPart-MonitorChartPart-497080ff-3f3b-4555-9c34-2d55afa301aa",
              "StartboardPart-MonitorChartPart-497080ff-3f3b-4555-9c34-2d55afa301ac",
              "StartboardPart-MonitorChartPart-497080ff-3f3b-4555-9c34-2d55afa301ae",
              "StartboardPart-MonitorChartPart-497080ff-3f3b-4555-9c34-2d55afa301b0",
              "StartboardPart-MonitorChartPart-497080ff-3f3b-4555-9c34-2d55afa305c6"
            ]
          }
        }
      }
    }
  },
  "name": "MSI-${environment}-monitoring-dashboard",
  "type": "Microsoft.Portal/dashboards",
  "location": "INSERT LOCATION",
  "tags": {
    "hidden-title": "MSI-${environment}-monitoring-dashboard"
  },
  "apiVersion": "2015-08-01-preview"
}
